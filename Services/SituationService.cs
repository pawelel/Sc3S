﻿using Microsoft.EntityFrameworkCore;
using Sc3S.Data;
using Sc3S.DTO;
using Sc3S.Entities;
using Sc3S.Exceptions;

namespace Sc3S.Services;


public interface ISituationService
{
    Task<(int, int)> CreateAssetSituation(int assetId, int situationId);

    Task<(int, int)> CreateCategorySituation(int categoryId, int situationId);

    Task<(int, int)> CreateDeviceSituation(int deviceId, int situationId);

    Task<int> CreateQuestion(QuestionCreateDto questionCreateDto);

    Task<int> CreateSituation(SituationCreateDto situationCreateDto);

    Task<(int, int)> CreateSituationDetail(int situationId, int detailId);

    Task<(int, int)> CreateSituationParameter(int situationId, int parameterId);

    Task<(int, int)> CreateSituationQuestion(int situationId, int questionId);

    Task DeleteAssetSituation(int assetId, int situationId);

    Task DeleteCategorySituation(int categoryId, int situationId);

    Task DeleteDeviceSituation(int deviceId, int situationId);

    Task DeleteQuestion(int situationId);

    Task DeleteSituation(int situationId);

    Task DeleteSituationDetail(int situationId, int detailId);

    Task DeleteSituationParameter(int situationId, int parameterId);

    Task DeleteSituationQuestion(int situationId, int questionId);

    Task<QuestionDto> GetQuestionById(int questionId);

    Task<IEnumerable<QuestionDto>> GetQuestions();

    Task<SituationDto> GetSituationById(int situationId);

    Task<IEnumerable<SituationDto>> GetSituations();

    Task<IEnumerable<SituationWithAssetsDto>> GetSituationsWithAssets();

    Task<IEnumerable<SituationWithCategoriesDto>> GetSituationsWithCategories();

    Task<IEnumerable<SituationWithQuestionsDto>> GetSituationsWithQuestions();

    Task<IEnumerable<SituationWithAssetsAndDetailsDto>> GetSituationWithAssetsAndDetails();

    Task MarkDeleteAssetSituation(int assetId, int situationId);

    Task MarkDeleteCategorySituation(int categoryId, int situationId);

    Task MarkDeleteDeviceSituation(int deviceId, int situationId);

    Task MarkDeleteQuestion(int questionId);

    Task MarkDeleteSituation(int situationId);

    Task MarkDeleteSituationDetail(int situationId, int detailId);

    Task MarkDeleteSituationParameter(int situationId, int parameterId);

    Task MarkDeleteSituationQuestion(int situationId, int questionId);

    Task UpdateAssetSituation(int assetId, int situationId);
    Task UpdateCategorySituation(int categoryId, int situationId);
    Task UpdateDeviceSituation(int deviceId, int situationId);
    Task UpdateQuestion(int questionId, QuestionUpdateDto questionUpdateDto);

    Task UpdateSituation(int questionId, SituationUpdateDto situationUpdateDto);

    Task UpdateSituationDetail(int situationId, int detailId);
    Task UpdateSituationParameter(int situationId, int parameterId);
    Task UpdateSituationQuestion(int situationId, int questionId);
}
public class SituationService : ISituationService
{
    private readonly IDbContextFactory<Sc3SContext> _factory;
    private readonly ILogger<SituationService> _logger;

    public SituationService(IDbContextFactory<Sc3SContext> factory, ILogger<SituationService> logger)
    {
        _factory = factory;
        _logger = logger;
    }
    public async Task<(int, int)> CreateAssetSituation(int assetId, int situationId)
    {

        await using var _context = await _factory.CreateDbContextAsync();

        // get assetSituation
        var assetSituation = await _context.AssetSituations.FindAsync(assetId, situationId);
        if (assetSituation != null)
            throw new BadRequestException("AssetSituation already exists");
        var asset = await _context.Assets.FindAsync(assetId);
        if (asset == null || asset.IsDeleted)
        {
            _logger.LogWarning("Asset not found");
            throw new NotFoundException("Asset not found");
        }
        var situation = await _context.Situations.FindAsync(situationId);
        if (situation == null || situation.IsDeleted)
        {
            _logger.LogWarning("Situation not found");
            throw new NotFoundException("Situation not found");
        }
        assetSituation = new AssetSituation
        {
            AssetId = assetId,
            SituationId = situationId,
            IsDeleted = false
        };
        _context.Add(assetSituation);
        // save changes

        try
        {
            await _context.SaveChangesAsync();

            return (assetId, situationId);
        }
        catch (Exception ex)
        {

            _logger.LogError(ex, "Error creating assetSituation");
            throw new BadRequestException("Error while saving changes");
        }
    }

    public async Task<(int, int)> CreateCategorySituation(int categoryId, int situationId)
    {

        await using var _context = await _factory.CreateDbContextAsync();

        // get categorySituation
        var categorySituation = await _context.CategorySituations.FindAsync(categoryId, situationId);
        if (categorySituation != null)
            throw new BadRequestException("CategorySituation already exists");
        var category = await _context.Categories.FindAsync(categoryId);
        if (category == null || category.IsDeleted)
        {
            _logger.LogWarning("Category not found");
            throw new NotFoundException("Category not found");
        }
        var situation = await _context.Situations.FindAsync(situationId);
        if (situation == null || situation.IsDeleted)
        {
            _logger.LogWarning("Situation not found");
            throw new NotFoundException("Situation not found");
        }
        categorySituation = new CategorySituation
        {
            CategoryId = categoryId,
            SituationId = situationId,

            IsDeleted = false
        };
        _context.Add(categorySituation);
        // save changes

        try
        {
            await _context.SaveChangesAsync();

            return (categoryId, situationId);
        }
        catch (Exception ex)
        {

            _logger.LogError(ex, "Error creating categorySituation");
            throw new BadRequestException("Error while saving changes");
        }
    }

    public async Task<(int, int)> CreateDeviceSituation(int deviceId, int situationId)
    {

        await using var _context = await _factory.CreateDbContextAsync();

        // get deviceSituation
        var deviceSituation = await _context.DeviceSituations.FindAsync(deviceId, situationId);
        if (deviceSituation != null)
            throw new BadRequestException("DeviceSituation already exists");
        var device = await _context.Devices.FindAsync(deviceId);
        if (device == null || device.IsDeleted)
        {
            _logger.LogWarning("Device not found");
            throw new NotFoundException("Device not found");
        }
        var situation = await _context.Situations.FindAsync(situationId);
        if (situation == null || situation.IsDeleted)
        {
            _logger.LogWarning("Situation not found");
            throw new NotFoundException("Situation not found");
        }
        deviceSituation = new DeviceSituation
        {
            DeviceId = deviceId,
            SituationId = situationId,

            IsDeleted = false
        };
        _context.Add(deviceSituation);
        // save changes

        try
        {
            await _context.SaveChangesAsync();

            return (deviceId, situationId);
        }
        catch (Exception ex)
        {

            _logger.LogError(ex, "Error creating deviceSituation");
            throw new BadRequestException("Error while saving changes");
        }
    }

    public async Task<int> CreateQuestion(QuestionCreateDto questionCreateDto)
    {

        await using var _context = await _factory.CreateDbContextAsync();


        // validate question name
        var duplicate = await _context.Questions.AnyAsync(c => c.Name.ToLower().Trim() == questionCreateDto.Name.ToLower().Trim());
        if (duplicate)
        {
            _logger.LogWarning("Question name already exists");
            throw new BadRequestException("Question name already exists");
        }

        var question = new Question
        {

            Name = questionCreateDto.Name,
            IsDeleted = false
        };
        // create question
        _context.Questions.Add(question);
        // await using transaction

        try
        {
            // save changes
            await _context.SaveChangesAsync();
            // commit transaction

            _logger.LogInformation("Question with id {QuestionId} created", question.QuestionId);
            return question.QuestionId;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating question");
            // rollback transaction

            throw new BadRequestException("Error creating question");
        }
    }

    public async Task<int> CreateSituation(SituationCreateDto situationCreateDto)
    {

        await using var _context = await _factory.CreateDbContextAsync();


        // validate situation name
        var duplicate = await _context.Situations.AnyAsync(c => c.Name.ToLower().Trim() == situationCreateDto.Name.ToLower().Trim());
        if (duplicate)
        {
            _logger.LogWarning("Situation name already exists");
            throw new BadRequestException("Situation name already exists");
        }

        var situation = new Situation
        {

            Name = situationCreateDto.Name,
            Description = situationCreateDto.Description,
            IsDeleted = false
        };
        // create situation
        _context.Situations.Add(situation);
        // await using transaction

        try
        {
            // save changes
            await _context.SaveChangesAsync();
            // commit transaction

            _logger.LogInformation("Situation with id {SituationId} created", situation.SituationId);
            return situation.SituationId;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating situation");
            // rollback transaction

            throw new BadRequestException("Error creating situation");
        }
    }

    public async Task<(int, int)> CreateSituationDetail(int situationId, int detailId)
    {

        await using var _context = await _factory.CreateDbContextAsync();

        // get situationDetail
        var situationDetail = await _context.SituationDetails.FindAsync(situationId, detailId);
        if (situationDetail != null)
            throw new BadRequestException("SituationDetail already exists");
        var situation = await _context.Situations.FindAsync(situationId);
        if (situation == null || situation.IsDeleted)
        {
            _logger.LogWarning("Situation not found");
            throw new NotFoundException("Situation not found");
        }
        var detail = await _context.Details.FindAsync(detailId);
        if (detail == null || detail.IsDeleted)
        {
            _logger.LogWarning("Detail not found");
            throw new NotFoundException("Detail not found");
        }
        situationDetail = new SituationDetail
        {
            SituationId = situationId,
            DetailId = detailId,

            IsDeleted = false
        };
        _context.Add(situationDetail);
        // save changes

        try
        {
            await _context.SaveChangesAsync();

            return (situationId, detailId);
        }
        catch (Exception ex)
        {

            _logger.LogError(ex, "Error creating situationDetail");
            throw new BadRequestException("Error while saving changes");
        }
    }

    public async Task<(int, int)> CreateSituationParameter(int situationId, int parameterId)
    {

        await using var _context = await _factory.CreateDbContextAsync();

        // get situationParameter
        var situationParameter = await _context.SituationParameters.FindAsync(situationId, parameterId);
        if (situationParameter != null)
            throw new BadRequestException("SituationParameter already exists");
        var situation = await _context.Situations.FindAsync(situationId);
        if (situation == null || situation.IsDeleted)
        {
            _logger.LogWarning("Situation not found");
            throw new NotFoundException("Situation not found");
        }
        var parameter = await _context.Parameters.FindAsync(parameterId);
        if (parameter == null || parameter.IsDeleted)
        {
            _logger.LogWarning("Parameter not found");
            throw new NotFoundException("Parameter not found");
        }
        situationParameter = new SituationParameter
        {
            SituationId = situationId,
            ParameterId = parameterId,

            IsDeleted = false
        };
        _context.Add(situationParameter);
        // save changes

        try
        {
            await _context.SaveChangesAsync();

            return (situationId, parameterId);
        }
        catch (Exception ex)
        {

            _logger.LogError(ex, "Error creating situationParameter");
            throw new BadRequestException("Error while saving changes");
        }
    }

    public async Task<(int, int)> CreateSituationQuestion(int situationId, int questionId)
    {

        await using var _context = await _factory.CreateDbContextAsync();

        // get situationQuestion
        var situationQuestion = await _context.SituationQuestions.FindAsync(situationId, questionId);
        if (situationQuestion != null)
            throw new BadRequestException("SituationQuestion already exists");
        var situation = await _context.Situations.FindAsync(situationId);
        if (situation == null || situation.IsDeleted)
        {
            _logger.LogWarning("Situation not found");
            throw new NotFoundException("Situation not found");
        }
        var question = await _context.Questions.FindAsync(questionId);
        if (question == null || question.IsDeleted)
        {
            _logger.LogWarning("Question not found");
            throw new NotFoundException("Question not found");
        }
        situationQuestion = new SituationQuestion
        {
            SituationId = situationId,
            QuestionId = questionId,

            IsDeleted = false
        };
        _context.Add(situationQuestion);
        // save changes

        try
        {
            await _context.SaveChangesAsync();

            return (situationId, questionId);
        }
        catch (Exception ex)
        {

            _logger.LogError(ex, "Error creating situationQuestion");
            throw new BadRequestException("Error while saving changes");
        }
    }

    public async Task DeleteAssetSituation(int assetId, int situationId)
    {
        await using var _context = await _factory.CreateDbContextAsync();


        // get asset situation
        var assetSituation = await _context.AssetSituations.FindAsync(assetId, situationId);
        if (assetSituation == null)
        {
            _logger.LogWarning("Asset situation not found");
            throw new NotFoundException("Asset situation not found");
        }
        // check if AssetSituation is not marked as deleted
        if (assetSituation.IsDeleted == false)
        {
            _logger.LogWarning("Asset situation is not marked as deleted");
            throw new BadRequestException("Asset situation is not marked as deleted");
        }
        // delete asset situation
        _context.AssetSituations.Remove(assetSituation);
        // await using transaction

        try
        {
            // save changes
            await _context.SaveChangesAsync();
            // commit transaction

            _logger.LogInformation("Asset situation with id {AssetId}, {SituationId}  deleted", assetId, situationId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting asset situation");
            // rollback transaction

            throw new BadRequestException("Error deleting asset situation");
        }
    }

    public async Task DeleteCategorySituation(int categoryId, int situationId)
    {
        await using var _context = await _factory.CreateDbContextAsync();


        // get category situation
        var categorySituation = await _context.CategorySituations.FindAsync(categoryId, situationId);
        if (categorySituation == null)
        {
            _logger.LogWarning("Category situation not found");
            throw new NotFoundException("Category situation not found");
        }
        // check if CategorySituation is not marked as deleted
        if (categorySituation.IsDeleted == false)
        {
            _logger.LogWarning("Category situation is not marked as deleted");
            throw new BadRequestException("Category situation is not marked as deleted");
        }
        // delete category situation
        _context.CategorySituations.Remove(categorySituation);
        // await using transaction

        try
        {
            // save changes
            await _context.SaveChangesAsync();
            // commit transaction

            _logger.LogInformation("Category situation with id {CategoryId}, {SituationId}  deleted", categoryId, situationId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting category situation");
            // rollback transaction

            throw new BadRequestException("Error deleting category situation");
        }
    }

    public async Task DeleteDeviceSituation(int deviceId, int situationId)
    {
        await using var _context = await _factory.CreateDbContextAsync();


        // get device situation
        var deviceSituation = await _context.DeviceSituations.FindAsync(deviceId, situationId);
        if (deviceSituation == null)
        {
            _logger.LogWarning("Device situation not found");
            throw new NotFoundException("Device situation not found");
        }
        // check if DeviceSituation is not marked as deleted
        if (deviceSituation.IsDeleted == false)
        {
            _logger.LogWarning("Device situation is not marked as deleted");
            throw new BadRequestException("Device situation is not marked as deleted");
        }
        // delete device situation
        _context.DeviceSituations.Remove(deviceSituation);
        // await using transaction

        try
        {
            // save changes
            await _context.SaveChangesAsync();
            // commit transaction

            _logger.LogInformation("Device situation with id {DeviceId}, {SituationId}  deleted", deviceId, situationId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting device situation");
            // rollback transaction

            throw new BadRequestException("Error deleting device situation");
        }
    }

    public async Task DeleteQuestion(int situationId)
    {
        await using var _context = await _factory.CreateDbContextAsync();


        // get question
        var question = await _context.Questions.FindAsync(situationId);
        if (question == null)
        {
            _logger.LogWarning("Question not found");
            throw new NotFoundException("Question not found");
        }
        // check if question is marked as deleted
        if (question.IsDeleted == false)
        {
            _logger.LogWarning("Question is not marked as deleted");
            throw new BadRequestException("Question is not marked as deleted");
        }
        // delete question
        _context.Questions.Remove(question);
        // await using transaction

        try
        {
            // save changes
            await _context.SaveChangesAsync();
            // commit transaction

            _logger.LogInformation("Question with id {QuestionId} deleted", question.QuestionId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting question");
            // rollback transaction

            throw new BadRequestException("Error deleting question");
        }
    }

    public async Task DeleteSituation(int situationId)
    {
        await using var _context = await _factory.CreateDbContextAsync();


        // get situation
        var situation = await _context.Situations.FindAsync(situationId);
        if (situation == null)
        {
            _logger.LogWarning("Situation not found");
            throw new NotFoundException("Situation not found");
        }
        // check if situation is marked as deleted
        if (situation.IsDeleted == false)
        {
            _logger.LogWarning("Situation is not marked as deleted");
            throw new BadRequestException("Situation is not marked as deleted");
        }
        // delete situation
        _context.Situations.Remove(situation);
        // await using transaction

        try
        {
            // save changes
            await _context.SaveChangesAsync();
            // commit transaction

            _logger.LogInformation("Situation with id {SituationId} deleted", situationId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting situation");
            // rollback transaction

            throw new BadRequestException("Error deleting situation");
        }
    }

    public async Task DeleteSituationDetail(int situationId, int detailId)
    {
        await using var _context = await _factory.CreateDbContextAsync();


        // get situation detail
        var situationDetail = await _context.SituationDetails.FindAsync(situationId, detailId);
        if (situationDetail == null)
        {
            _logger.LogWarning("Situation detail not found");
            throw new NotFoundException("Situation detail not found");
        }
        // check if situation detail is marked as deleted
        if (situationDetail.IsDeleted == false)
        {
            _logger.LogWarning("Situation detail is not marked as deleted");
            throw new BadRequestException("Situation detail is not marked as deleted");
        }
        // delete situation detail
        _context.SituationDetails.Remove(situationDetail);
        // await using transaction

        try
        {
            // save changes
            await _context.SaveChangesAsync();
            // commit transaction

            _logger.LogInformation("Situation detail with id {SituationId}, {DetailId} deleted", situationId, detailId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting situation detail");
            // rollback transaction

            throw new BadRequestException("Error deleting situation detail");
        }
    }

    public async Task DeleteSituationParameter(int situationId, int parameterId)
    {
        await using var _context = await _factory.CreateDbContextAsync();


        // get situation parameter
        var situationParameter = await _context.SituationParameters.FindAsync(situationId, parameterId);
        if (situationParameter == null)
        {
            _logger.LogWarning("Situation parameter not found");
            throw new NotFoundException("Situation parameter not found");
        }
        // check if situation parameter is marked as deleted
        if (situationParameter.IsDeleted == false)
        {
            _logger.LogWarning("Situation parameter is not marked as deleted");
            throw new BadRequestException("Situation parameter is not marked as deleted");
        }
        // delete situation parameter
        _context.SituationParameters.Remove(situationParameter);
        // await using transaction

        try
        {
            // save changes
            await _context.SaveChangesAsync();
            // commit transaction

            _logger.LogInformation("Situation parameter with id {SituationId}, {ParameterId} deleted", situationId, parameterId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting situation parameter");
            // rollback transaction

            throw new BadRequestException("Error deleting situation parameter");
        }
    }

    public async Task DeleteSituationQuestion(int situationId, int questionId)
    {
        await using var _context = await _factory.CreateDbContextAsync();


        // get situation question
        var situationQuestion = await _context.SituationQuestions.FindAsync(situationId, questionId);
        if (situationQuestion == null)
        {
            _logger.LogWarning("Situation question not found");
            throw new NotFoundException("Situation question not found");
        }
        // check if situation question is marked as deleted
        if (situationQuestion.IsDeleted == false)
        {
            _logger.LogWarning("Situation question is not marked as deleted");
            throw new BadRequestException("Situation question is not marked as deleted");
        }
        // delete situation question
        _context.SituationQuestions.Remove(situationQuestion);
        // await using transaction

        try
        {
            // save changes
            await _context.SaveChangesAsync();
            // commit transaction

            _logger.LogInformation("Situation question with id {SituationId}, {QuestionId} deleted", situationId, questionId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting situation question");
            // rollback transaction

            throw new BadRequestException("Error deleting situation question");
        }
    }

    public async Task<QuestionDto> GetQuestionById(int questionId)
    {
        await using var _context = await _factory.CreateDbContextAsync();


        // get question
        var question = await _context.Questions
            .AsNoTracking()
            .Select(q => new QuestionDto
            {
                QuestionId = q.QuestionId,
                Name = q.Name,
                IsDeleted = q.IsDeleted,
                UserId = q.UpdatedBy
            })
            .FirstOrDefaultAsync(c => c.QuestionId == questionId);
        if (question == null)
        {
            _logger.LogWarning("Question not found");
            throw new NotFoundException("Question not found");
        }
        // return question
        _logger.LogInformation("Question with id {QuestionId} returned", questionId);
        return question;
    }

    public async Task<IEnumerable<QuestionDto>> GetQuestions()
    {
        await using var _context = await _factory.CreateDbContextAsync();


        // get questions
        var questions = await _context.Questions
            .AsNoTracking()
            .Select(q => new QuestionDto
            {
                QuestionId = q.QuestionId,
                Name = q.Name,
                IsDeleted = q.IsDeleted,
                UserId = q.UpdatedBy
            })
            .ToListAsync();
        if (questions is null)
        {
            _logger.LogWarning("Questions not found");
            throw new NotFoundException("Questions not found");
        }
        // return questions
        _logger.LogInformation("Questions returned");
        return questions;
    }

    public async Task<SituationDto> GetSituationById(int situationId)
    {
        await using var _context = await _factory.CreateDbContextAsync();


        // get situation
        var situation = await _context.Situations
            .AsNoTracking()
            .Select(s => new SituationDto
            {
                SituationId = s.SituationId,
                Name = s.Name,
                Description = s.Description,
                IsDeleted = s.IsDeleted,
                UserId = s.UpdatedBy
            })
            .FirstOrDefaultAsync(c => c.SituationId == situationId);
        if (situation == null)
        {
            _logger.LogWarning("Situation not found");
            throw new NotFoundException("Situation not found");
        }
        // return situation
        _logger.LogInformation("Situation with id {SituationId} returned", situationId);
        return situation;
    }

    public async Task<IEnumerable<SituationDto>> GetSituations()
    {
        await using var _context = await _factory.CreateDbContextAsync();


        // get situations
        var situations = await _context.Situations
            .AsNoTracking()
            .Select(s => new SituationDto
            {
                SituationId = s.SituationId,
                Name = s.Name,
                Description = s.Description,
                IsDeleted = s.IsDeleted,
                UserId = s.UpdatedBy
            })
            .ToListAsync();
        if (situations is null)
        {
            _logger.LogWarning("Situations not found");
            throw new NotFoundException("Situations not found");
        }
        // return situations
        _logger.LogInformation("Situations returned");
        return situations;
    }

    public async Task<IEnumerable<SituationWithAssetsDto>> GetSituationsWithAssets()
    {
        await using var _context = await _factory.CreateDbContextAsync();


        // get situations with assets
        var situations = await _context.Situations
            .AsNoTracking()
            .Select(s => new SituationWithAssetsDto
            {
                Name = s.Name,
                Description = s.Description,
                SituationId = s.SituationId,
                IsDeleted = s.IsDeleted,
                UserId = s.UpdatedBy,
                Assets = s.AssetSituations.Select(a => new AssetDto
                {
                    AssetId = a.AssetId,
                    Name = a.Asset.Name,
                    Description = a.Asset.Description,
                    IsDeleted = a.Asset.IsDeleted,
                    UserId = a.Asset.UpdatedBy
                }).ToList()
            }).ToListAsync();
        if (situations is null)
        {
            _logger.LogWarning("Situations not found");
            throw new NotFoundException("Situations not found");
        }
        // return situations
        _logger.LogInformation("Situations returned");
        return situations;
    }

    public async Task<IEnumerable<SituationWithCategoriesDto>> GetSituationsWithCategories()
    {
        await using var _context = await _factory.CreateDbContextAsync();


        // get situations with categories
        var situations = await _context.Situations
            .AsNoTracking()
            .Select(s => new SituationWithCategoriesDto
            {
                Name = s.Name,
                Description = s.Description,
                SituationId = s.SituationId,
                IsDeleted = s.IsDeleted,
                UserId = s.UpdatedBy,
                Categories = s.CategorySituations.Select(c => new CategoryDto
                {
                    CategoryId = c.CategoryId,
                    Name = c.Category.Name,
                    Description = c.Category.Description,
                    IsDeleted = c.Category.IsDeleted,
                    UserId = c.Category.UpdatedBy
                }).ToList()
            }).ToListAsync();
        if (situations is null)
        {
            _logger.LogWarning("Situations not found");
            throw new NotFoundException("Situations not found");
        }
        // return situations
        _logger.LogInformation("Situations returned");
        return situations;
    }

    public async Task<IEnumerable<SituationWithQuestionsDto>> GetSituationsWithQuestions()
    {
        await using var _context = await _factory.CreateDbContextAsync();


        // get situations with questions
        var situations = await _context.Situations
            .AsNoTracking()
            .Select(s => new SituationWithQuestionsDto
            {
                Name = s.Name,
                Description = s.Description,
                SituationId = s.SituationId,
                IsDeleted = s.IsDeleted,
                UserId = s.UpdatedBy,
                Questions = s.SituationQuestions.Select(q => new QuestionDto
                {
                    QuestionId = q.QuestionId,
                    IsDeleted = q.Question.IsDeleted,
                    UserId = q.Question.UpdatedBy
                }).ToList()
            }).ToListAsync();
        if (situations is null)
        {
            _logger.LogWarning("Situations not found");
            throw new NotFoundException("Situations not found");
        }
        // return situations
        _logger.LogInformation("Situations returned");
        return situations;
    }

    public async Task<IEnumerable<SituationWithAssetsAndDetailsDto>> GetSituationWithAssetsAndDetails()
    {
        await using var _context = await _factory.CreateDbContextAsync();


        // get situations with assets and details
        var situations = await _context.Situations
            .AsNoTracking()
            .Select(s => new SituationWithAssetsAndDetailsDto
            {
                Name = s.Name,
                Description = s.Description,
                SituationId = s.SituationId,
                IsDeleted = s.IsDeleted,
                UserId = s.UpdatedBy,
                Assets = s.AssetSituations.Select(a => new AssetWithDetailsDisplayDto
                {
                    AssetId = a.AssetId,
                    Name = a.Asset.Name,
                    Description = a.Asset.Description,
                    IsDeleted = a.Asset.IsDeleted,
                    UserId = a.Asset.UpdatedBy,
                    Details = a.Asset.AssetDetails.Select(d => new AssetDetailDisplayDto
                    {
                        Name = d.Detail.Name,
                        Description = d.Detail.Description,
                        IsDeleted = d.Detail.IsDeleted,
                        UserId = d.Detail.UpdatedBy
                    }).ToList()
                }).ToList()
            }).ToListAsync();
        // if (situations is null)
        if (situations is null)
        {
            _logger.LogWarning("Situations with asset details not found");
            throw new NotFoundException("Situations with asset details not found");
        }
        // return situations
        _logger.LogInformation("Situations with asset details returned");
        return situations;
    }

    public async Task MarkDeleteAssetSituation(int assetId, int situationId)
    {

        await using var _context = await _factory.CreateDbContextAsync();


        // get assetSituation
        var assetSituation = await _context.AssetSituations.FindAsync(assetId, situationId);
        if (assetSituation == null)
        {
            _logger.LogWarning("AssetSituation not found");
            throw new NotFoundException("AssetSituation not found");
        }
        if (assetSituation.IsDeleted)
        {
            _logger.LogWarning("AssetSituation already marked as deleted");
            throw new BadRequestException("AssetSituation already marked as deleted");
        }
        assetSituation.IsDeleted = true;
        _context.Update(assetSituation);
        // await using transaction

        try
        {
            // save changes
            await _context.SaveChangesAsync();

            _logger.LogInformation("AssetSituation with id {AssetId}, {SituationId} marked as deleted", assetId, situationId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error marking assetSituation with id {AssetId}, {SituationId} as deleted", assetId, situationId);

            throw new BadRequestException($"Error marking assetSituation with id {assetId}, {situationId} as deleted");
        }
    }

    public async Task MarkDeleteCategorySituation(int categoryId, int situationId)
    {

        await using var _context = await _factory.CreateDbContextAsync();


        // get categorySituation
        var categorySituation = await _context.CategorySituations.FindAsync(categoryId, situationId);
        if (categorySituation == null)
        {
            _logger.LogWarning("CategorySituation not found");
            throw new NotFoundException("CategorySituation not found");
        }
        if (categorySituation.IsDeleted)
        {
            _logger.LogWarning("CategorySituation already marked as deleted");
            throw new BadRequestException("CategorySituation already marked as deleted");
        }
        categorySituation.IsDeleted = true;
        _context.Update(categorySituation);
        // await using transaction

        try
        {
            // save changes
            await _context.SaveChangesAsync();

            _logger.LogInformation("CategorySituation with id {CategoryId}, {SituationId} marked as deleted", categoryId, situationId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error marking categorySituation with id {CategoryId}, {SituationId} as deleted", categoryId, situationId);

            throw new BadRequestException($"Error marking categorySituation with id {categoryId}, {situationId} as deleted");
        }
    }

    public async Task MarkDeleteDeviceSituation(int deviceId, int situationId)
    {

        await using var _context = await _factory.CreateDbContextAsync();


        // get deviceSituation
        var deviceSituation = await _context.DeviceSituations.FindAsync(deviceId, situationId);
        if (deviceSituation == null)
        {
            _logger.LogWarning("DeviceSituation not found");
            throw new NotFoundException("DeviceSituation not found");
        }
        if (deviceSituation.IsDeleted)
        {
            _logger.LogWarning("DeviceSituation already marked as deleted");
            throw new BadRequestException("DeviceSituation already marked as deleted");
        }
        deviceSituation.IsDeleted = true;
        _context.Update(deviceSituation);
        // await using transaction

        try
        {
            // save changes
            await _context.SaveChangesAsync();

            _logger.LogInformation("DeviceSituation with id {DeviceId}, {SituationId} marked as deleted", deviceId, situationId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error marking deviceSituation with id {DeviceId}, {SituationId} as deleted", deviceId, situationId);

            throw new BadRequestException($"Error marking deviceSituation with id {deviceId}, {situationId} as deleted");
        }
    }

    public async Task MarkDeleteQuestion(int questionId)
    {

        await using var _context = await _factory.CreateDbContextAsync();


        // get question
        var question = await _context.Questions
            .Include(q => q.SituationQuestions)
            .FirstOrDefaultAsync(q => q.QuestionId == questionId);
        // if question not found
        if (question == null)
        {
            _logger.LogWarning("Question with id {QuestionId} not found", questionId);
            throw new NotFoundException("Question not found");
        }
        // if question is already deleted
        if (question.IsDeleted)
        {
            _logger.LogWarning("Question with id {QuestionId} is already deleted", questionId);
            throw new BadRequestException("Question is already deleted");
        }
        // check if question has SituationQuestions with IsDeleted = false
        if (question.SituationQuestions.Any(sq => sq.IsDeleted == false))
        {
            _logger.LogWarning("Question with id {QuestionId} has SituationQuestions with IsDeleted = false", questionId);
            throw new BadRequestException("Question has SituationQuestions with IsDeleted = false");
        }

        // mark question as deleted
        question.IsDeleted = true;
        // await using transaction

        try
        {
            // save changes
            await _context.SaveChangesAsync();
            // commit transaction

            // return success
            _logger.LogInformation("Question with id {QuestionId} marked as deleted", questionId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error marking question with id {QuestionId} as deleted", questionId);
            // rollback transaction

            throw new BadRequestException($"Error marking question with id {questionId} as deleted");
        }
    }

    public async Task MarkDeleteSituation(int situationId)
    {

        await using var _context = await _factory.CreateDbContextAsync();


        // get situation
        var situation = await _context.Situations
            .Include(s => s.SituationQuestions)
            .Include(s => s.AssetSituations)
            .Include(s => s.CategorySituations)
            .Include(s => s.DeviceSituations)
            .Include(s => s.SituationDetails)
            .Include(s => s.SituationParameters)
            .FirstOrDefaultAsync(s => s.SituationId == situationId);
        // if situation not found
        if (situation == null)
        {
            _logger.LogWarning("Situation with id {SituationId} not found", situationId);
            throw new NotFoundException("Situation not found");
        }
        // if situation is already deleted
        if (situation.IsDeleted)
        {
            _logger.LogWarning("Situation with id {SituationId} is already deleted", situationId);
            throw new BadRequestException("Situation is already deleted");
        }
        // check if situation has SituationQuestions with IsDeleted = false
        if (situation.SituationQuestions.Any(sq => sq.IsDeleted == false))
        {
            _logger.LogWarning("Situation with id {SituationId} has SituationQuestions with IsDeleted = false", situationId);
            throw new BadRequestException("Situation has SituationQuestions with IsDeleted = false");
        }
        // check if situation has SituationDetails with IsDeleted = false
        if (situation.SituationDetails.Any(sd => sd.IsDeleted == false))
        {
            _logger.LogWarning("Situation with id {SituationId} has SituationDetails with IsDeleted = false", situationId);
            throw new BadRequestException("Situation has SituationDetails with IsDeleted = false");
        }
        // check if situation has SituationParameters with IsDeleted = false
        if (situation.SituationParameters.Any(sp => sp.IsDeleted == false))
        {
            _logger.LogWarning("Situation with id {SituationId} has SituationParameters with IsDeleted = false", situationId);
            throw new BadRequestException("Situation has SituationParameters with IsDeleted = false");
        }
        // check if situation has AssetSituations with IsDeleted = false
        if (situation.AssetSituations.Any(asit => asit.IsDeleted == false))
        {
            _logger.LogWarning("Situation with id {SituationId} has AssetSituations with IsDeleted = false", situationId);
            throw new BadRequestException("Situation has AssetSituations with IsDeleted = false");
        }
        // check if situation has CategorySituations with IsDeleted = false
        if (situation.CategorySituations.Any(cs => cs.IsDeleted == false))
        {
            _logger.LogWarning("Situation with id {SituationId} has CategorySituations with IsDeleted = false", situationId);
            throw new BadRequestException("Situation has CategorySituations with IsDeleted = false");
        }
        // check if situation has DeviceSituations with IsDeleted = false
        if (situation.DeviceSituations.Any(ds => ds.IsDeleted == false))
        {
            _logger.LogWarning("Situation with id {SituationId} has DeviceSituations with IsDeleted = false", situationId);
            throw new BadRequestException("Situation has DeviceSituations with IsDeleted = false");
        }
        // mark delete situation
        situation.IsDeleted = true;
        _context.Situations.Update(situation);
        // await using transaction

        try
        {
            // save changes
            await _context.SaveChangesAsync();
            // await commit transaction

            _logger.LogInformation("Situation with id {SituationId} is marked as deleted", situationId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while marking as deleted situation with id {SituationId}", situationId);
            // await rollback transaction


            throw new BadRequestException($"Error while marking as deleted situation with id {situationId}");
        }
    }

    public async Task MarkDeleteSituationDetail(int situationId, int detailId)
    {

        await using var _context = await _factory.CreateDbContextAsync();


        // get situationDetail
        var situationDetail = await _context.SituationDetails.FindAsync(situationId, detailId);
        if (situationDetail == null)
        {
            _logger.LogWarning("SituationDetail not found");
            throw new NotFoundException("SituationDetail not found");
        }
        if (situationDetail.IsDeleted)
        {
            _logger.LogWarning("SituationDetail already marked as deleted");
            throw new BadRequestException("SituationDetail already marked as deleted");
        }

        situationDetail.IsDeleted = true;
        _context.Update(situationDetail);
        // await using transaction

        try
        {
            // save changes
            await _context.SaveChangesAsync();

            _logger.LogInformation("SituationDetail with id {SituationId}, {DetailId} marked as deleted", situationId, detailId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error marking situationDetail with id {SituationId}, {DetailId} as deleted", situationId, detailId);

            throw new BadRequestException($"Error marking situationDetail with id {situationId}, {detailId} as deleted");
        }
    }

    public async Task MarkDeleteSituationParameter(int situationId, int parameterId)
    {

        await using var _context = await _factory.CreateDbContextAsync();


        // get situationParameter
        var situationParameter = await _context.SituationParameters.FindAsync(situationId, parameterId);
        if (situationParameter == null)
        {
            _logger.LogWarning("SituationParameter not found");
            throw new NotFoundException("SituationParameter not found");
        }
        if (situationParameter.IsDeleted)
        {
            _logger.LogWarning("SituationParameter already marked as deleted");
            throw new BadRequestException("SituationParameter already marked as deleted");
        }
        situationParameter.IsDeleted = true;
        _context.Update(situationParameter);
        // await using transaction

        try
        {
            // save changes
            await _context.SaveChangesAsync();

            _logger.LogInformation("SituationParameter with id {SituationId}, {ParameterId} marked as deleted", situationId, parameterId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error marking situationParameter with id {SituationId}, {ParameterId} as deleted", situationId, parameterId);

            throw new BadRequestException($"Error marking situationParameter with id {situationId}, {parameterId} as deleted");
        }
    }

    public async Task MarkDeleteSituationQuestion(int situationId, int questionId)
    {

        await using var _context = await _factory.CreateDbContextAsync();


        // get situationQuestion
        var situationQuestion = await _context.SituationQuestions.FindAsync(situationId, questionId);
        if (situationQuestion == null)
        {
            _logger.LogWarning("SituationQuestion not found");
            throw new NotFoundException("SituationQuestion not found");
        }
        if (situationQuestion.IsDeleted)
        {
            _logger.LogWarning("SituationQuestion already marked as deleted");
            throw new BadRequestException("SituationQuestion already marked as deleted");
        }
        situationQuestion.IsDeleted = true;
        _context.Update(situationQuestion);
        // await using transaction

        try
        {
            // save changes
            await _context.SaveChangesAsync();

            _logger.LogInformation("SituationQuestion with id {SituationId}, {QuestionId} marked as deleted", situationId, questionId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error marking situationQuestion with id {SituationId}, {QuestionId} as deleted", situationId, questionId);

            throw new BadRequestException($"Error marking situationQuestion with id {situationId}, {questionId} as deleted");
        }
    }

    public async Task UpdateAssetSituation(int assetId, int situationId)
    {

        await using var _context = await _factory.CreateDbContextAsync();

        // get assetSituation
        var assetSituation = await _context.AssetSituations.FindAsync(assetId, situationId);
        if (assetSituation == null)
        {
            _logger.LogWarning("AssetSituation not found");
            throw new NotFoundException("AssetSituation not found");
        }
        if (!assetSituation.IsDeleted)
            throw new BadRequestException("AssetSituation not marked as deleted");
        var asset = await _context.Assets.FindAsync(assetId);
        if (asset == null || asset.IsDeleted)
        {
            _logger.LogWarning("Asset not found");
            throw new NotFoundException("Asset not found");
        }
        var situation = await _context.Situations.FindAsync(situationId);
        if (situation == null || situation.IsDeleted)
        {
            _logger.LogWarning("Situation not found");
            throw new NotFoundException("Situation not found");
        }
        assetSituation.IsDeleted = false;
        // save changes

        try
        {
            await _context.SaveChangesAsync();

        }
        catch (Exception ex)
        {

            _logger.LogError(ex, "Error updating assetSituation");
            throw new BadRequestException("Error while saving changes");
        }
    }

    public async Task UpdateCategorySituation(int categoryId, int situationId)
    {

        await using var _context = await _factory.CreateDbContextAsync();

        // get categorySituation
        var categorySituation = await _context.CategorySituations.FindAsync(categoryId, situationId);
        if (categorySituation == null)
        {
            _logger.LogWarning("CategorySituation not found");
            throw new NotFoundException("CategorySituation not found");
        }
        if (!categorySituation.IsDeleted)
            throw new BadRequestException("CategorySituation not marked as deleted");
        var category = await _context.Categories.FindAsync(categoryId);
        if (category == null || category.IsDeleted)
        {
            _logger.LogWarning("Category not found");
            throw new NotFoundException("Category not found");
        }
        var situation = await _context.Situations.FindAsync(situationId);
        if (situation == null || situation.IsDeleted)
        {
            _logger.LogWarning("Situation not found");
            throw new NotFoundException("Situation not found");
        }
        categorySituation.IsDeleted = false;
        // save changes

        try
        {
            await _context.SaveChangesAsync();

        }
        catch (Exception ex)
        {

            _logger.LogError(ex, "Error updating categorySituation");
            throw new BadRequestException("Error while saving changes");
        }
    }

    public async Task UpdateDeviceSituation(int deviceId, int situationId)
    {

        await using var _context = await _factory.CreateDbContextAsync();

        // get deviceSituation
        var deviceSituation = await _context.DeviceSituations.FindAsync(deviceId, situationId);
        if (deviceSituation == null)
        {
            _logger.LogWarning("DeviceSituation not found");
            throw new NotFoundException("DeviceSituation not found");
        }
        if (!deviceSituation.IsDeleted)
            throw new BadRequestException("DeviceSituation not marked as deleted");
        var device = await _context.Devices.FindAsync(deviceId);
        if (device == null || device.IsDeleted)
        {
            _logger.LogWarning("Device not found");
            throw new NotFoundException("Device not found");
        }
        var situation = await _context.Situations.FindAsync(situationId);
        if (situation == null || situation.IsDeleted)
        {
            _logger.LogWarning("Situation not found");
            throw new NotFoundException("Situation not found");
        }
        deviceSituation.IsDeleted = false;
        // save changes

        try
        {
            await _context.SaveChangesAsync();

        }
        catch (Exception ex)
        {

            _logger.LogError(ex, "Error updating deviceSituation");
            throw new BadRequestException("Error while saving changes");
        }
    }

    public async Task UpdateQuestion(int questionId, QuestionUpdateDto questionUpdateDto)
    {

        await using var _context = await _factory.CreateDbContextAsync();


        // get question
        var question = await _context.Questions.FirstOrDefaultAsync(m => m.QuestionId == questionId);
        if (question == null)
        {
            _logger.LogWarning("Question not found");
            throw new NotFoundException("Question not found");
        }
        // check if question name from dto is already taken
        var duplicate = await _context.Questions.AnyAsync(a => a.Name.ToLower().Trim() == questionUpdateDto.Name.ToLower().Trim());
        if (duplicate || question.Name.ToLower().Trim() == questionUpdateDto.Name.ToLower().Trim())
        {
            _logger.LogWarning("Question name is already taken");
            throw new BadRequestException("Question name is already taken");
        }

        question.Name = questionUpdateDto.Name;
        // assign userId to update
        question.IsDeleted = false;
        // update question
        _context.Update(question);
        // await using transaction

        try
        {
            // save changes
            await _context.SaveChangesAsync();
            // await commit transaction

            // return success
            _logger.LogInformation("Question updated");
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error updating question");
            // await rollback transaction

            throw new BadRequestException("Error updating question");
        }
    }

    public async Task UpdateSituation(int situationId, SituationUpdateDto situationUpdateDto)
    {

        await using var _context = await _factory.CreateDbContextAsync();


        // get situation
        var situation = await _context.Situations.FirstOrDefaultAsync(m => m.SituationId == situationId);
        if (situation == null)
        {
            _logger.LogWarning("Situation not found");
            throw new NotFoundException("Situation not found");
        }
        // check if situation name from dto is already taken
        var duplicate = await _context.Situations.AnyAsync(a => a.Name.ToLower().Trim() == situationUpdateDto.Name.ToLower().Trim());
        if (duplicate || situation.Name.ToLower().Trim() == situationUpdateDto.Name.ToLower().Trim())
        {
            _logger.LogWarning("Situation name is already taken");
            throw new BadRequestException("Situation name is already taken");
        }

        situation.Name = situationUpdateDto.Name;
        // assign userId to update
        situation.IsDeleted = false;
        // update situation
        _context.Update(situation);
        // await using transaction

        try
        {
            // save changes
            await _context.SaveChangesAsync();
            // await commit transaction

            // return success
            _logger.LogInformation("Situation updated");
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error updating situation");
            // await rollback transaction

            throw new BadRequestException("Error updating situation");
        }
    }

    public async Task UpdateSituationDetail(int situationId, int detailId)
    {

        await using var _context = await _factory.CreateDbContextAsync();

        // get situationDetail
        var situationDetail = await _context.SituationDetails.FindAsync(situationId, detailId);
        if (situationDetail == null)
        {
            _logger.LogWarning("SituationDetail not found");
            throw new NotFoundException("SituationDetail not found");
        }
        if (!situationDetail.IsDeleted)
            throw new BadRequestException("SituationDetail not marked as deleted");
        var situation = await _context.Situations.FindAsync(situationId);
        if (situation == null || situation.IsDeleted)
        {
            _logger.LogWarning("Situation not found");
            throw new NotFoundException("Situation not found");
        }
        var detail = await _context.Details.FindAsync(detailId);
        if (detail == null || detail.IsDeleted)
        {
            _logger.LogWarning("Detail not found");
            throw new NotFoundException("Detail not found");
        }
        situationDetail.IsDeleted = false;
        // save changes

        try
        {
            await _context.SaveChangesAsync();

        }
        catch (Exception ex)
        {

            _logger.LogError(ex, "Error updating situationDetail");
            throw new BadRequestException("Error while saving changes");
        }
    }

    public async Task UpdateSituationParameter(int situationId, int parameterId)
    {

        await using var _context = await _factory.CreateDbContextAsync();

        // get situationParameter
        var situationParameter = await _context.SituationParameters.FindAsync(situationId, parameterId);
        if (situationParameter == null)
        {
            _logger.LogWarning("SituationParameter not found");
            throw new NotFoundException("SituationParameter not found");
        }
        if (!situationParameter.IsDeleted)
            throw new BadRequestException("SituationParameter not marked as deleted");
        var situation = await _context.Situations.FindAsync(situationId);
        if (situation == null || situation.IsDeleted)
        {
            _logger.LogWarning("Situation not found");
            throw new NotFoundException("Situation not found");
        }
        var parameter = await _context.Parameters.FindAsync(parameterId);
        if (parameter == null || parameter.IsDeleted)
        {
            _logger.LogWarning("Parameter not found");
            throw new NotFoundException("Parameter not found");
        }
        situationParameter.IsDeleted = false;
        // save changes

        try
        {
            await _context.SaveChangesAsync();

        }
        catch (Exception ex)
        {

            _logger.LogError(ex, "Error updating situationParameter");
            throw new BadRequestException("Error while saving changes");
        }
    }

    public async Task UpdateSituationQuestion(int situationId, int questionId)
    {

        await using var _context = await _factory.CreateDbContextAsync();

        // get situationQuestion
        var situationQuestion = await _context.SituationQuestions.FindAsync(situationId, questionId);
        if (situationQuestion == null)
        {
            _logger.LogWarning("SituationQuestion not found");
            throw new NotFoundException("SituationQuestion not found");
        }
        if (!situationQuestion.IsDeleted)
            throw new BadRequestException("SituationQuestion not marked as deleted");
        var situation = await _context.Situations.FindAsync(situationId);
        if (situation == null || situation.IsDeleted)
        {
            _logger.LogWarning("Situation not found");
            throw new NotFoundException("Situation not found");
        }
        var question = await _context.Questions.FindAsync(questionId);
        if (question == null || question.IsDeleted)
        {
            _logger.LogWarning("Question not found");
            throw new NotFoundException("Question not found");
        }
        situationQuestion.IsDeleted = false;
        // save changes

        try
        {
            await _context.SaveChangesAsync();

        }
        catch (Exception ex)
        {

            _logger.LogError(ex, "Error updating situationQuestion");
            throw new BadRequestException("Error while saving changes");
        }
    }
}
