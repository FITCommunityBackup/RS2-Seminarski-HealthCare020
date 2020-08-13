﻿using HealthCare020.Services.Interfaces;
using HealthCare020.Services.Properties;
using Microsoft.Azure.CognitiveServices.Vision.Face;
using Microsoft.Azure.CognitiveServices.Vision.Face.Models;
using NLog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace HealthCare020.Services.Services
{
    public class FaceRecognitionService : IFaceRecognitionService
    {
        private string RecognitionModel = "recognition_02";
        private readonly IFaceClient _faceClinet;
        private readonly ILogger _logger;

        public FaceRecognitionService()
        {
            _logger = LogManager.GetCurrentClassLogger();
            _faceClinet = new FaceClient(new ApiKeyServiceClientCredentials(Resources.AzureFaceAPI_Key)) { Endpoint = Resources.AzureFaceAPI_Endpoint };
        }

        public async Task<IList<Person>> GetPersonGroupPersonsList(string personGroupId, int pageSize=6,string startPersonId="")
        {
            try
            {
                var result = await _faceClinet.PersonGroupPerson.ListWithHttpMessagesAsync(personGroupId,start:startPersonId,top:pageSize);
                if (result.Response.StatusCode == HttpStatusCode.OK)
                {
                    return result.Body;
                }
                _logger.Error(await result.Response.Content.ReadAsStringAsync());
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }
            return new List<Person>();
        }

        public async Task<bool> CreatePersonGroup(string personGroupId, string groupName)
        {
            try
            {
                await _faceClinet.PersonGroup.CreateAsync(personGroupId, groupName, recognitionModel: RecognitionModel);
                return true;
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
                return false;
            }
        }

        public async Task<Person> CreatePersonInGroup(string personGroupId, string name)
        {
            var person = await _faceClinet.PersonGroupPerson.CreateAsync(personGroupId, name,$"Date and time created: {DateTime.Now:g}");

            return person;
        }

        public async Task<PersistedFace> AddFaceToPerson(string personGroupId, Guid personId, Stream stream)
        {
            try
            {
                var persistedFace =
                    await _faceClinet.PersonGroupPerson.AddFaceFromStreamWithHttpMessagesAsync(personGroupId, personId,
                        stream);
                if (!await TrainModel(personGroupId))
                    return null;
                return persistedFace.Body;
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
                return null;
            }
        }

        private async Task<bool> TrainModel(string personGroupId)
        {
            await _faceClinet.PersonGroup.TrainAsync(personGroupId);
            var trainingStatus = await _faceClinet.PersonGroup.GetTrainingStatusAsync(personGroupId);

            return trainingStatus.Status == TrainingStatusType.Succeeded;
        }

        public async Task<Guid?> IdentifyFace(Stream stream, string personGroupId)
        {
            try
            {
                var detectedFace = await DetectFace(stream);
                if (!detectedFace.FaceId.HasValue)
                    return null;

                var identificationResultResponse = (await _faceClinet.Face.IdentifyWithHttpMessagesAsync(
                    faceIds: new List<Guid> { detectedFace.FaceId.Value },
                    personGroupId,
                    confidenceThreshold: 0.5));

                var identificationResult = identificationResultResponse.Body?.FirstOrDefault();

                if (identificationResult == null)
                    return null;

                var maxConfidence = identificationResult.Candidates.Max(x => x.Confidence);

                return identificationResult?.Candidates
                    .FirstOrDefault(x => Math.Abs(x.Confidence - maxConfidence) < .001)?.PersonId;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<DetectedFace> DetectFace(Stream stream)
        {
            return (await _faceClinet.Face.DetectWithStreamAsync(stream, recognitionModel: RecognitionModel))?.FirstOrDefault();
        }

        public async Task<Person> GetPerson(Guid personId, string personGroupId)
        {
            return await _faceClinet.PersonGroupPerson.GetAsync(personGroupId, personId);
        }

        public async Task DeletePersonFromGroup(Guid personId, string personGroupId)
        {
            await _faceClinet.PersonGroupPerson.DeleteAsync(personGroupId, personId);
        }

        public async Task DeletePersonGroup(string personGroupId)
        {
            await _faceClinet.PersonGroup.DeleteAsync(personGroupId);
        }

        public async Task DeleteFacesFromPerson(string personGroupId, Guid personId)
        {
            var person = await _faceClinet.PersonGroupPerson.GetAsync(personGroupId, personId);
            foreach (var personPersistedFaceId in person.PersistedFaceIds)
            {
                await _faceClinet.PersonGroupPerson.DeleteFaceAsync(personGroupId, personId, personPersistedFaceId);
            }
        }
    }
}