using Microsoft.EntityFrameworkCore;

using Sc3S.CQRS.Commands;
using Sc3S.CQRS.Queries;
using Sc3S.Data;
using Sc3S.Entities;
using Sc3S.Exceptions;
using Sc3S.Helpers;

namespace Sc3S.Services;


public interface ISituationService
{
    Task<ServiceResponse<(int,int)>> CreateAssetSituation(int assetId, int situationId);

    Task<ServiceResponse<(int,int)>> CreateCategorySituation(int categoryId, int situationId);

    Task<ServiceResponse<(int,int)>> CreateDeviceSituation(int deviceId, int situationId);

    Task<ServiceResponse<int>> CreateQuestion(QuestionUpdateCommand questionUpdateDto);

    Task<ServiceResponse<int>> CreateSituation(SituationUpdateCommand situationUpdateDto);

    Task<ServiceResponse<(int,int)>> CreateSituationDetail(int situationId, int detailId);

    Task<ServiceResponse<(int,int)>> CreateSituationParameter(int situationId, int parameterId);

    Task<ServiceResponse<(int,int)>> CreateSituationQuestion(int situationId, int questionId);

    Task<ServiceResponse> DeleteAssetSituation(int assetId, int situationId);

    Task<ServiceResponse> DeleteCategorySituation(int categoryId, int situationId);

    Task<ServiceResponse> DeleteDeviceSituation(int deviceId, int situationId);

    Task<ServiceResponse> DeleteQuestion(int situationId);

    Task<ServiceResponse> DeleteSituation(int situationId);

    Task<ServiceResponse> DeleteSituationDetail(int situationId, int detailId);

    Task<ServiceResponse> DeleteSituationParameter(int situationId, int parameterId);

    Task<ServiceResponse> DeleteSituationQuestion(int situationId, int questionId);

    Task<ServiceResponse<QuestionQuery>> GetQuestionById(int questionId);

    Task<ServiceResponse<IEnumerable<QuestionQuery>>> GetQuestions();

    Task<ServiceResponse<SituationQuery>> GetSituationById(int situationId);

    Task<ServiceResponse<IEnumerable<SituationQuery>>> GetSituations();

    Task<ServiceResponse<IEnumerable<SituationWithAssetsQuery>>> GetSituationsWithAssets();

    Task<ServiceResponse<IEnumerable<SituationWithCategoriesQuery>>> GetSituationsWithCategories();

    Task<ServiceResponse<IEnumerable<SituationWithQuestionsQuery>>> GetSituationsWithQuestions();

    Task<ServiceResponse<IEnumerable<SituationWithAssetsAndDetailsQuery>>> GetSituationWithAssetsAndDetails();

    Task<ServiceResponse> MarkDeleteAssetSituation(int assetId, int situationId);

    Task<ServiceResponse> MarkDeleteCategorySituation(int categoryId, int situationId);

    Task<ServiceResponse> MarkDeleteDeviceSituation(int deviceId, int situationId);

    Task<ServiceResponse> MarkDeleteQuestion(int questionId);

    Task<ServiceResponse> MarkDeleteSituation(int situationId);

    Task<ServiceResponse> MarkDeleteSituationDetail(int situationId, int detailId);

    Task<ServiceResponse> MarkDeleteSituationParameter(int situationId, int parameterId);

    Task<ServiceResponse> MarkDeleteSituationQuestion(int situationId, int questionId);

    Task<ServiceResponse> UpdateAssetSituation(int assetId, int situationId);
    Task<ServiceResponse> UpdateCategorySituation(int categoryId, int situationId);
    Task<ServiceResponse> UpdateDeviceSituation(int deviceId, int situationId);
    Task<ServiceResponse> UpdateQuestion(int questionId, QuestionUpdateCommand questionUpdateDto);

    Task<ServiceResponse> UpdateSituation(int questionId, SituationUpdateCommand situationUpdateDto);

    Task<ServiceResponse> UpdateSituationDetail(int situationId, int detailId);
    Task<ServiceResponse> UpdateSituationParameter(int situationId, int parameterId);
    Task<ServiceResponse> UpdateSituationQuestion(int situationId, int questionId);
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
    public async Task<ServiceResponse<(int,int)>> CreateAssetSituation(int assetId, int situationId)
    {

        await using var _context = await _factory.CreateDbContextAsync();

        // get assetSituation
        var assetSituation = await _context.AssetSituations.FindAsync(assetId, situationId);
        if (assetSituation != null)
            return new ServiceResponse(null, "AssetSituation already exists");
        var asset = await _context.Assets.FindAsync(assetId);
        if (asset == null || asset.IsDeleted)
        {
            _logger.LogWarning("Asset not found");
            return new ServiceResponse("Asset not found");
        }
        var situation = await _context.Situations.FindAsync(situationId);
        if (situation == null || situation.IsDeleted)
        {
            _logger.LogWarning("Situation not found");
            return new ServiceResponse("Situation not found");
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
            return new ServiceResponse("Error while saving changes");
        }
    }

    public async Task<ServiceResponse<(int,int)>> CreateCategorySituation(int categoryId, int situationId)
    {

        await using var _context = await _factory.CreateDbContextAsync();

        // get categorySituation
        var categorySituation = await _context.CategorySituations.FindAsync(categoryId, situationId);
        if (categorySituation != null)
            return new ServiceResponse("CategorySituation already exists");
        var category = await _context.Categories.FindAsync(categoryId);
        if (category == null || category.IsDeleted)
        {
            _logger.LogWarning("Category not found");
            return new ServiceResponse("Category not found");
        }
        var situation = await _context.Situations.FindAsync(situationId);
        if (situation == null || situation.IsDeleted)
        {
            _logger.LogWarning("Situation not found");
            return new ServiceResponse("Situation not found");
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
            return new ServiceResponse("Error while saving changes");
        }
    }

    public async Task<ServiceResponse<(int,int)>> CreateDeviceSituation(int deviceId, int situationId)
    {

        await using var _context = await _factory.CreateDbContextAsync();

        // get deviceSituation
        var deviceSituation = await _context.DeviceSituations.FindAsync(deviceId, situationId);
        if (deviceSituation != null)
            return new ServiceResponse("DeviceSituation already exists");
        var device = await _context.Devices.FindAsync(deviceId);
        if (device == null || device.IsDeleted)
        {
            _logger.LogWarning("Device not found");
            return new ServiceResponse("Device not found");
        }
        var situation = await _context.Situations.FindAsync(situationId);
        if (situation == null || situation.IsDeleted)
        {
            _logger.LogWarning("Situation not found");
            return new ServiceResponse("Situation not found");
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
            return new ServiceResponse("Error while saving changes");
        }
    }

    public async Task<ServiceResponse<int>> CreateQuestion(QuestionUpdateCommand questionUpdateDto)
    {

        await using var _context = await _factory.CreateDbContextAsync();


        // validate question name
        var duplicate = await _context.Questions.AnyAsync(c => c.Name.ToLower().Trim() == questionUpdateDto.Name.ToLower().Trim());
        if (duplicate)
        {
            _logger.LogWarning("Question name already exists");
            return new ServiceResponse("Question name already exists");
        }

        var question = new Question
        {

            Name = questionUpdateDto.Name,
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

            return new ServiceResponse("Error creating question");
        }
    }

    public async Task<ServiceResponse<int>> CreateSituation(SituationUpdateCommand situationUpdateDto)
    {

        await using var _context = await _factory.CreateDbContextAsync();


        // validate situation name
        var duplicate = await _context.Situations.AnyAsync(c => c.Name.ToLower().Trim() == situationUpdateDto.Name.ToLower().Trim());
        if (duplicate)
        {
            _logger.LogWarning("Situation name already exists");
            return new ServiceResponse("Situation name already exists");
        }

        var situation = new Situation
        {

            Name = situationUpdateDto.Name,
            Description = situationUpdateDto.Description,
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

            return new ServiceResponse("Error creating situation");
        }
    }

    public async Task<ServiceResponse<(int,int)>> CreateSituationDetail(int situationId, int detailId)
    {

        await using var _context = await _factory.CreateDbContextAsync();

        // get situationDetail
        var situationDetail = await _context.SituationDetails.FindAsync(situationId, detailId);
        if (situationDetail != null)
            return new ServiceResponse("SituationDetail already exists");
        var situation = await _context.Situations.FindAsync(situationId);
        if (situation == null || situation.IsDeleted)
        {
            _logger.LogWarning("Situation not found");
            return new ServiceResponse("Situation not found");
        }
        var detail = await _context.Details.FindAsync(detailId);
        if (detail == null || detail.IsDeleted)
        {
            _logger.LogWarning("Detail not found");
            return new ServiceResponse("Detail not found");
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
            return new ServiceResponse("Error while saving changes");
        }
    }

    public async Task<ServiceResponse<(int,int)>> CreateSituationParameter(int situationId, int parameterId)
    {

        await using var _context = await _factory.CreateDbContextAsync();

        // get situationParameter
        var situationParameter = await _context.SituationParameters.FindAsync(situationId, parameterId);
        if (situationParameter != null)
            return new ServiceResponse("SituationParameter already exists");
        var situation = await _context.Situations.FindAsync(situationId);
        if (situation == null || situation.IsDeleted)
        {
            _logger.LogWarning("Situation not found");
            return new ServiceResponse("Situation not found");
        }
        var parameter = await _context.Parameters.FindAsync(parameterId);
        if (parameter == null || parameter.IsDeleted)
        {
            _logger.LogWarning("Parameter not found");
            return new ServiceResponse("Parameter not found");
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
            return new ServiceResponse("Error while saving changes");
        }
    }

    public async Task<ServiceResponse<(int,int)>> CreateSituationQuestion(int situationId, int questionId)
    {

        await using var _context = await _factory.CreateDbContextAsync();

        // get situationQuestion
        var situationQuestion = await _context.SituationQuestions.FindAsync(situationId, questionId);
        if (situationQuestion != null)
            return new ServiceResponse("SituationQuestion already exists");
        var situation = await _context.Situations.FindAsync(situationId);
        if (situation == null || situation.IsDeleted)
        {
            _logger.LogWarning("Situation not found");
            return new ServiceResponse("Situation not found");
        }
        var question = await _context.Questions.FindAsync(questionId);
        if (question == null || question.IsDeleted)
        {
            _logger.LogWarning("Question not found");
            return new ServiceResponse("Question not found");
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
            return new ServiceResponse("Error while saving changes");
        }
    }

    public async Task<ServiceResponse> DeleteAssetSituation(int assetId, int situationId)
    {
        await using var _context = await _factory.CreateDbContextAsync();


        // get asset situation
        var assetSituation = await _context.AssetSituations.FindAsync(assetId, situationId);
        if (assetSituation == null)
        {
            _logger.LogWarning("Asset situation not found");
            return new ServiceResponse("Asset situation not found");
        }
        // check if AssetSituation is not marked as deleted
        if (assetSituation.IsDeleted == false)
        {
            _logger.LogWarning("Asset situation is not marked as deleted");
            return new ServiceResponse("Asset situation is not marked as deleted");
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

            return new ServiceResponse("Error deleting asset situation");
        }
    }

    public async Task<ServiceResponse> DeleteCategorySituation(int categoryId, int situationId)
    {
        await using var _context = await _factory.CreateDbContextAsync();


        // get category situation
        var categorySituation = await _context.CategorySituations.FindAsync(categoryId, situationId);
        if (categorySituation == null)
        {
            _logger.LogWarning("Category situation not found");
            return new ServiceResponse("Category situation not found");
        }
        // check if CategorySituation is not marked as deleted
        if (categorySituation.IsDeleted == false)
        {
            _logger.LogWarning("Category situation is not marked as deleted");
            return new ServiceResponse("Category situation is not marked as deleted");
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

            return new ServiceResponse("Error deleting category situation");
        }
    }

    public async Task<ServiceResponse> DeleteDeviceSituation(int deviceId, int situationId)
    {
        await using var _context = await _factory.CreateDbContextAsync();


        // get device situation
        var deviceSituation = await _context.DeviceSituations.FindAsync(deviceId, situationId);
        if (deviceSituation == null)
        {
            _logger.LogWarning("Device situation not found");
            return new ServiceResponse("Device situation not found");
        }
        // check if DeviceSituation is not marked as deleted
        if (deviceSituation.IsDeleted == false)
        {
            _logger.LogWarning("Device situation is not marked as deleted");
            return new ServiceResponse("Device situation is not marked as deleted");
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

            return new ServiceResponse("Error deleting device situation");
        }
    }

    public async Task<ServiceResponse> DeleteQuestion(int situationId)
    {
        await using var _context = await _factory.CreateDbContextAsync();


        // get question
        var question = await _context.Questions.FindAsync(situationId);
        if (question == null)
        {
            _logger.LogWarning("Question not found");
            return new ServiceResponse("Question not found");
        }
        // check if question is marked as deleted
        if (question.IsDeleted == false)
        {
            _logger.LogWarning("Question is not marked as deleted");
            return new ServiceResponse("Question is not marked as deleted");
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

            return new ServiceResponse("Error deleting question");
        }
    }

    public async Task<ServiceResponse> DeleteSituation(int situationId)
    {
        await using var _context = await _factory.CreateDbContextAsync();


        // get situation
        var situation = await _context.Situations.FindAsync(situationId);
        if (situation == null)
        {
            _logger.LogWarning("Situation not found");
            return new ServiceResponse("Situation not found");
        }
        // check if situation is marked as deleted
        if (situation.IsDeleted == false)
        {
            _logger.LogWarning("Situation is not marked as deleted");
            return new ServiceResponse("Situation is not marked as deleted");
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

            return new ServiceResponse("Error deleting situation");
        }
    }

    public async Task<ServiceResponse> DeleteSituationDetail(int situationId, int detailId)
    {
        await using var _context = await _factory.CreateDbContextAsync();


        // get situation detail
        var situationDetail = await _context.SituationDetails.FindAsync(situationId, detailId);
        if (situationDetail == null)
        {
            _logger.LogWarning("Situation detail not found");
            return new ServiceResponse("Situation detail not found");
        }
        // check if situation detail is marked as deleted
        if (situationDetail.IsDeleted == false)
        {
            _logger.LogWarning("Situation detail is not marked as deleted");
            return new ServiceResponse("Situation detail is not marked as deleted");
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

            return new ServiceResponse("Error deleting situation detail");
        }
    }

    public async Task<ServiceResponse> DeleteSituationParameter(int situationId, int parameterId)
    {
        await using var _context = await _factory.CreateDbContextAsync();


        // get situation parameter
        var situationParameter = await _context.SituationParameters.FindAsync(situationId, parameterId);
        if (situationParameter == null)
        {
            _logger.LogWarning("Situation parameter not found");
            return new ServiceResponse("Situation parameter not found");
        }
        // check if situation parameter is marked as deleted
        if (situationParameter.IsDeleted == false)
        {
            _logger.LogWarning("Situation parameter is not marked as deleted");
            return new ServiceResponse("Situation parameter is not marked as deleted");
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

            return new ServiceResponse("Error deleting situation parameter");
        }
    }

    public async Task<ServiceResponse> DeleteSituationQuestion(int situationId, int questionId)
    {
        await using var _context = await _factory.CreateDbContextAsync();


        // get situation question
        var situationQuestion = await _context.SituationQuestions.FindAsync(situationId, questionId);
        if (situationQuestion == null)
        {
            _logger.LogWarning("Situation question not found");
            return new ServiceResponse("Situation question not found");
        }
        // check if situation question is marked as deleted
        if (situationQuestion.IsDeleted == false)
        {
            _logger.LogWarning("Situation question is not marked as deleted");
            return new ServiceResponse("Situation question is not marked as deleted");
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

            return new ServiceResponse("Error deleting situation question");
        }
    }

    public async Task<ServiceResponse<QuestionQuery>> GetQuestionById(int questionId)
    {
        await using var _context = await _factory.CreateDbContextAsync();


        // get question
        var question = await _context.Questions
            .AsNoTracking()
            .Select(q => new QuestionQuery
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
            return new ServiceResponse("Question not found");
        }
        // return question
        _logger.LogInformation("Question with id {QuestionId} returned", questionId);
        return question;
    }

    public async Task<ServiceResponse<IEnumerable<QuestionQuery>>> GetQuestions()
    {
        await using var _context = await _factory.CreateDbContextAsync();


        // get questions
        var questions = await _context.Questions
            .AsNoTracking()
            .Select(q => new QuestionQuery
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
            return new ServiceResponse("Questions not found");
        }
        // return questions
        _logger.LogInformation("Questions returned");
        return questions;
    }

    public async Task<ServiceResponse<SituationQuery>> GetSituationById(int situationId)
    {
        await using var _context = await _factory.CreateDbContextAsync();


        // get situation
        var situation = await _context.Situations
            .AsNoTracking()
            .Select(s => new SituationQuery
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
            return new ServiceResponse("Situation not found");
        }
        // return situation
        _logger.LogInformation("Situation with id {SituationId} returned", situationId);
        return situation;
    }

    public async Task<ServiceResponse<IEnumerable<SituationQuery>>> GetSituations()
    {
        await using var _context = await _factory.CreateDbContextAsync();


        // get situations
        var situations = await _context.Situations
            .AsNoTracking()
            .Select(s => new SituationQuery
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
            return new ServiceResponse("Situations not found");
        }
        // return situations
        _logger.LogInformation("Situations returned");
        return situations;
    }

    public async Task<ServiceResponse<IEnumerable<SituationWithAssetsQuery>>> GetSituationsWithAssets()
    {
        await using var _context = await _factory.CreateDbContextAsync();


        // get situations with assets
        var situations = await _context.Situations
            .AsNoTracking()
            .Select(s => new SituationWithAssetsQuery
            {
                Name = s.Name,
                Description = s.Description,
                SituationId = s.SituationId,
                IsDeleted = s.IsDeleted,
                UserId = s.UpdatedBy,
                Assets = s.AssetSituations.Select(a => new AssetQuery
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
            return new ServiceResponse("Situations not found");
        }
        // return situations
        _logger.LogInformation("Situations returned");
        return situations;
    }

    public async Task<ServiceResponse<IEnumerable<SituationWithCategoriesQuery>>> GetSituationsWithCategories()
    {
        await using var _context = await _factory.CreateDbContextAsync();


        // get situations with categories
        var situations = await _context.Situations
            .AsNoTracking()
            .Select(s => new SituationWithCategoriesQuery
            {
                Name = s.Name,
                Description = s.Description,
                SituationId = s.SituationId,
                IsDeleted = s.IsDeleted,
                UserId = s.UpdatedBy,
                Categories = s.CategorySituations.Select(c => new CategoryQuery
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
            return new ServiceResponse("Situations not found");
        }
        // return situations
        _logger.LogInformation("Situations returned");
        return situations;
    }

    public async Task<ServiceResponse<IEnumerable<SituationWithQuestionsQuery>>> GetSituationsWithQuestions()
    {
        await using var _context = await _factory.CreateDbContextAsync();


        // get situations with questions
        var situations = await _context.Situations
            .AsNoTracking()
            .Select(s => new SituationWithQuestionsQuery
            {
                Name = s.Name,
                Description = s.Description,
                SituationId = s.SituationId,
                IsDeleted = s.IsDeleted,
                UserId = s.UpdatedBy,
                Questions = s.SituationQuestions.Select(q => new QuestionQuery
                {
                    QuestionId = q.QuestionId,
                    IsDeleted = q.Question.IsDeleted,
                    UserId = q.Question.UpdatedBy
                }).ToList()
            }).ToListAsync();
        if (situations is null)
        {
            _logger.LogWarning("Situations not found");
            return new ServiceResponse("Situations not found");
        }
        // return situations
        _logger.LogInformation("Situations returned");
        return situations;
    }

    public async Task<ServiceResponse<IEnumerable<SituationWithAssetsAndDetailsQuery>>> GetSituationWithAssetsAndDetails()
    {
        await using var _context = await _factory.CreateDbContextAsync();


        // get situations with assets and details
        var situations = await _context.Situations
            .AsNoTracking()
            .Select(s => new SituationWithAssetsAndDetailsQuery
            {
                Name = s.Name,
                Description = s.Description,
                SituationId = s.SituationId,
                IsDeleted = s.IsDeleted,
                UserId = s.UpdatedBy,
                Assets = s.AssetSituations.Select(a => new AssetWithDetailsDisplayQuery
                {
                    AssetId = a.AssetId,
                    Name = a.Asset.Name,
                    Description = a.Asset.Description,
                    IsDeleted = a.Asset.IsDeleted,
                    UserId = a.Asset.UpdatedBy,
                    Details = a.Asset.AssetDetails.Select(d => new AssetDetailDisplayQuery
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
            return new ServiceResponse("Situations with asset details not found");
        }
        // return situations
        _logger.LogInformation("Situations with asset details returned");
        return situations;
    }

    public async Task<ServiceResponse> MarkDeleteAssetSituation(int assetId, int situationId)
    {

        await using var _context = await _factory.CreateDbContextAsync();


        // get assetSituation
        var assetSituation = await _context.AssetSituations.FindAsync(assetId, situationId);
        if (assetSituation == null)
        {
            _logger.LogWarning("AssetSituation not found");
            return new ServiceResponse("AssetSituation not found");
        }
        if (assetSituation.IsDeleted)
        {
            _logger.LogWarning("AssetSituation already marked as deleted");
            return new ServiceResponse("AssetSituation already marked as deleted");
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

            return new ServiceResponse($"Error marking assetSituation with id {assetId}, {situationId} as deleted");
        }
    }

    public async Task<ServiceResponse> MarkDeleteCategorySituation(int categoryId, int situationId)
    {

        await using var _context = await _factory.CreateDbContextAsync();


        // get categorySituation
        var categorySituation = await _context.CategorySituations.FindAsync(categoryId, situationId);
        if (categorySituation == null)
        {
            _logger.LogWarning("CategorySituation not found");
            return new ServiceResponse("CategorySituation not found");
        }
        if (categorySituation.IsDeleted)
        {
            _logger.LogWarning("CategorySituation already marked as deleted");
            return new ServiceResponse("CategorySituation already marked as deleted");
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

            return new ServiceResponse($"Error marking categorySituation with id {categoryId}, {situationId} as deleted");
        }
    }

    public async Task<ServiceResponse> MarkDeleteDeviceSituation(int deviceId, int situationId)
    {

        await using var _context = await _factory.CreateDbContextAsync();


        // get deviceSituation
        var deviceSituation = await _context.DeviceSituations.FindAsync(deviceId, situationId);
        if (deviceSituation == null)
        {
            _logger.LogWarning("DeviceSituation not found");
            return new ServiceResponse("DeviceSituation not found");
        }
        if (deviceSituation.IsDeleted)
        {
            _logger.LogWarning("DeviceSituation already marked as deleted");
            return new ServiceResponse("DeviceSituation already marked as deleted");
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

            return new ServiceResponse($"Error marking deviceSituation with id {deviceId}, {situationId} as deleted");
        }
    }

    public async Task<ServiceResponse> MarkDeleteQuestion(int questionId)
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
            return new ServiceResponse("Question not found");
        }
        // if question is already deleted
        if (question.IsDeleted)
        {
            _logger.LogWarning("Question with id {QuestionId} is already deleted", questionId);
            return new ServiceResponse("Question is already deleted");
        }
        // check if question has SituationQuestions with IsDeleted = false
        if (question.SituationQuestions.Any(sq => sq.IsDeleted == false))
        {
            _logger.LogWarning("Question with id {QuestionId} has SituationQuestions with IsDeleted = false", questionId);
            return new ServiceResponse("Question has SituationQuestions with IsDeleted = false");
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

            return new ServiceResponse($"Error marking question with id {questionId} as deleted");
        }
    }

    public async Task<ServiceResponse> MarkDeleteSituation(int situationId)
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
            return new ServiceResponse("Situation not found");
        }
        // if situation is already deleted
        if (situation.IsDeleted)
        {
            _logger.LogWarning("Situation with id {SituationId} is already deleted", situationId);
            return new ServiceResponse("Situation is already deleted");
        }
        // check if situation has SituationQuestions with IsDeleted = false
        if (situation.SituationQuestions.Any(sq => sq.IsDeleted == false))
        {
            _logger.LogWarning("Situation with id {SituationId} has SituationQuestions with IsDeleted = false", situationId);
            return new ServiceResponse("Situation has SituationQuestions with IsDeleted = false");
        }
        // check if situation has SituationDetails with IsDeleted = false
        if (situation.SituationDetails.Any(sd => sd.IsDeleted == false))
        {
            _logger.LogWarning("Situation with id {SituationId} has SituationDetails with IsDeleted = false", situationId);
            return new ServiceResponse("Situation has SituationDetails with IsDeleted = false");
        }
        // check if situation has SituationParameters with IsDeleted = false
        if (situation.SituationParameters.Any(sp => sp.IsDeleted == false))
        {
            _logger.LogWarning("Situation with id {SituationId} has SituationParameters with IsDeleted = false", situationId);
            return new ServiceResponse("Situation has SituationParameters with IsDeleted = false");
        }
        // check if situation has AssetSituations with IsDeleted = false
        if (situation.AssetSituations.Any(asit => asit.IsDeleted == false))
        {
            _logger.LogWarning("Situation with id {SituationId} has AssetSituations with IsDeleted = false", situationId);
            return new ServiceResponse("Situation has AssetSituations with IsDeleted = false");
        }
        // check if situation has CategorySituations with IsDeleted = false
        if (situation.CategorySituations.Any(cs => cs.IsDeleted == false))
        {
            _logger.LogWarning("Situation with id {SituationId} has CategorySituations with IsDeleted = false", situationId);
            return new ServiceResponse("Situation has CategorySituations with IsDeleted = false");
        }
        // check if situation has DeviceSituations with IsDeleted = false
        if (situation.DeviceSituations.Any(ds => ds.IsDeleted == false))
        {
            _logger.LogWarning("Situation with id {SituationId} has DeviceSituations with IsDeleted = false", situationId);
            return new ServiceResponse("Situation has DeviceSituations with IsDeleted = false");
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


            return new ServiceResponse($"Error while marking as deleted situation with id {situationId}");
        }
    }

    public async Task<ServiceResponse> MarkDeleteSituationDetail(int situationId, int detailId)
    {

        await using var _context = await _factory.CreateDbContextAsync();


        // get situationDetail
        var situationDetail = await _context.SituationDetails.FindAsync(situationId, detailId);
        if (situationDetail == null)
        {
            _logger.LogWarning("SituationDetail not found");
            return new ServiceResponse("SituationDetail not found");
        }
        if (situationDetail.IsDeleted)
        {
            _logger.LogWarning("SituationDetail already marked as deleted");
            return new ServiceResponse("SituationDetail already marked as deleted");
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

            return new ServiceResponse($"Error marking situationDetail with id {situationId}, {detailId} as deleted");
        }
    }

    public async Task<ServiceResponse> MarkDeleteSituationParameter(int situationId, int parameterId)
    {

        await using var _context = await _factory.CreateDbContextAsync();


        // get situationParameter
        var situationParameter = await _context.SituationParameters.FindAsync(situationId, parameterId);
        if (situationParameter == null)
        {
            _logger.LogWarning("SituationParameter not found");
            return new ServiceResponse("SituationParameter not found");
        }
        if (situationParameter.IsDeleted)
        {
            _logger.LogWarning("SituationParameter already marked as deleted");
            return new ServiceResponse("SituationParameter already marked as deleted");
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

            return new ServiceResponse($"Error marking situationParameter with id {situationId}, {parameterId} as deleted");
        }
    }

    public async Task<ServiceResponse> MarkDeleteSituationQuestion(int situationId, int questionId)
    {

        await using var _context = await _factory.CreateDbContextAsync();


        // get situationQuestion
        var situationQuestion = await _context.SituationQuestions.FindAsync(situationId, questionId);
        if (situationQuestion == null)
        {
            _logger.LogWarning("SituationQuestion not found");
            return new ServiceResponse("SituationQuestion not found");
        }
        if (situationQuestion.IsDeleted)
        {
            _logger.LogWarning("SituationQuestion already marked as deleted");
            return new ServiceResponse("SituationQuestion already marked as deleted");
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

            return new ServiceResponse($"Error marking situationQuestion with id {situationId}, {questionId} as deleted");
        }
    }

    public async Task<ServiceResponse> UpdateAssetSituation(int assetId, int situationId)
    {

        await using var _context = await _factory.CreateDbContextAsync();

        // get assetSituation
        var assetSituation = await _context.AssetSituations.FindAsync(assetId, situationId);
        if (assetSituation == null)
        {
            _logger.LogWarning("AssetSituation not found");
            return new ServiceResponse("AssetSituation not found");
        }
        if (!assetSituation.IsDeleted)
            return new ServiceResponse("AssetSituation not marked as deleted");
        var asset = await _context.Assets.FindAsync(assetId);
        if (asset == null || asset.IsDeleted)
        {
            _logger.LogWarning("Asset not found");
            return new ServiceResponse("Asset not found");
        }
        var situation = await _context.Situations.FindAsync(situationId);
        if (situation == null || situation.IsDeleted)
        {
            _logger.LogWarning("Situation not found");
            return new ServiceResponse("Situation not found");
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
            return new ServiceResponse("Error while saving changes");
        }
    }

    public async Task<ServiceResponse> UpdateCategorySituation(int categoryId, int situationId)
    {

        await using var _context = await _factory.CreateDbContextAsync();

        // get categorySituation
        var categorySituation = await _context.CategorySituations.FindAsync(categoryId, situationId);
        if (categorySituation == null)
        {
            _logger.LogWarning("CategorySituation not found");
            return new ServiceResponse("CategorySituation not found");
        }
        if (!categorySituation.IsDeleted)
            return new ServiceResponse("CategorySituation not marked as deleted");
        var category = await _context.Categories.FindAsync(categoryId);
        if (category == null || category.IsDeleted)
        {
            _logger.LogWarning("Category not found");
            return new ServiceResponse("Category not found");
        }
        var situation = await _context.Situations.FindAsync(situationId);
        if (situation == null || situation.IsDeleted)
        {
            _logger.LogWarning("Situation not found");
            return new ServiceResponse("Situation not found");
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
            return new ServiceResponse("Error while saving changes");
        }
    }

    public async Task<ServiceResponse> UpdateDeviceSituation(int deviceId, int situationId)
    {

        await using var _context = await _factory.CreateDbContextAsync();

        // get deviceSituation
        var deviceSituation = await _context.DeviceSituations.FindAsync(deviceId, situationId);
        if (deviceSituation == null)
        {
            _logger.LogWarning("DeviceSituation not found");
            return new ServiceResponse("DeviceSituation not found");
        }
        if (!deviceSituation.IsDeleted)
            return new ServiceResponse("DeviceSituation not marked as deleted");
        var device = await _context.Devices.FindAsync(deviceId);
        if (device == null || device.IsDeleted)
        {
            _logger.LogWarning("Device not found");
            return new ServiceResponse("Device not found");
        }
        var situation = await _context.Situations.FindAsync(situationId);
        if (situation == null || situation.IsDeleted)
        {
            _logger.LogWarning("Situation not found");
            return new ServiceResponse("Situation not found");
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
            return new ServiceResponse("Error while saving changes");
        }
    }

    public async Task<ServiceResponse> UpdateQuestion(int questionId, QuestionUpdateCommand questionUpdateDto)
    {

        await using var _context = await _factory.CreateDbContextAsync();


        // get question
        var question = await _context.Questions.FirstOrDefaultAsync(m => m.QuestionId == questionId);
        if (question == null)
        {
            _logger.LogWarning("Question not found");
            return new ServiceResponse("Question not found");
        }
        // check if question name from dto is already taken
        var duplicate = await _context.Questions.AnyAsync(a => a.Name.ToLower().Trim() == questionUpdateDto.Name.ToLower().Trim());
        if (duplicate || question.Name.ToLower().Trim() == questionUpdateDto.Name.ToLower().Trim())
        {
            _logger.LogWarning("Question name is already taken");
            return new ServiceResponse("Question name is already taken");
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

            return new ServiceResponse("Error updating question");
        }
    }

    public async Task<ServiceResponse> UpdateSituation(int situationId, SituationUpdateCommand situationUpdateDto)
    {

        await using var _context = await _factory.CreateDbContextAsync();


        // get situation
        var situation = await _context.Situations.FirstOrDefaultAsync(m => m.SituationId == situationId);
        if (situation == null)
        {
            _logger.LogWarning("Situation not found");
            return new ServiceResponse("Situation not found");
        }
        // check if situation name from dto is already taken
        var duplicate = await _context.Situations.AnyAsync(a => a.Name.ToLower().Trim() == situationUpdateDto.Name.ToLower().Trim());
        if (duplicate || situation.Name.ToLower().Trim() == situationUpdateDto.Name.ToLower().Trim())
        {
            _logger.LogWarning("Situation name is already taken");
            return new ServiceResponse("Situation name is already taken");
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

            return new ServiceResponse("Error updating situation");
        }
    }

    public async Task<ServiceResponse> UpdateSituationDetail(int situationId, int detailId)
    {

        await using var _context = await _factory.CreateDbContextAsync();

        // get situationDetail
        var situationDetail = await _context.SituationDetails.FindAsync(situationId, detailId);
        if (situationDetail == null)
        {
            _logger.LogWarning("SituationDetail not found");
            return new ServiceResponse("SituationDetail not found");
        }
        if (!situationDetail.IsDeleted)
            return new ServiceResponse("SituationDetail not marked as deleted");
        var situation = await _context.Situations.FindAsync(situationId);
        if (situation == null || situation.IsDeleted)
        {
            _logger.LogWarning("Situation not found");
            return new ServiceResponse("Situation not found");
        }
        var detail = await _context.Details.FindAsync(detailId);
        if (detail == null || detail.IsDeleted)
        {
            _logger.LogWarning("Detail not found");
            return new ServiceResponse("Detail not found");
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
            return new ServiceResponse("Error while saving changes");
        }
    }

    public async Task<ServiceResponse> UpdateSituationParameter(int situationId, int parameterId)
    {

        await using var _context = await _factory.CreateDbContextAsync();

        // get situationParameter
        var situationParameter = await _context.SituationParameters.FindAsync(situationId, parameterId);
        if (situationParameter == null)
        {
            _logger.LogWarning("SituationParameter not found");
            return new ServiceResponse("SituationParameter not found");
        }
        if (!situationParameter.IsDeleted)
            return new ServiceResponse("SituationParameter not marked as deleted");
        var situation = await _context.Situations.FindAsync(situationId);
        if (situation == null || situation.IsDeleted)
        {
            _logger.LogWarning("Situation not found");
            return new ServiceResponse("Situation not found");
        }
        var parameter = await _context.Parameters.FindAsync(parameterId);
        if (parameter == null || parameter.IsDeleted)
        {
            _logger.LogWarning("Parameter not found");
            return new ServiceResponse("Parameter not found");
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
            return new ServiceResponse("Error while saving changes");
        }
    }

    public async Task<ServiceResponse> UpdateSituationQuestion(int situationId, int questionId)
    {

        await using var _context = await _factory.CreateDbContextAsync();

        // get situationQuestion
        var situationQuestion = await _context.SituationQuestions.FindAsync(situationId, questionId);
        if (situationQuestion == null)
        {
            _logger.LogWarning("SituationQuestion not found");
            return new ServiceResponse("SituationQuestion not found");
        }
        if (!situationQuestion.IsDeleted)
            return new ServiceResponse("SituationQuestion not marked as deleted");
        var situation = await _context.Situations.FindAsync(situationId);
        if (situation == null || situation.IsDeleted)
        {
            _logger.LogWarning("Situation not found");
            return new ServiceResponse("Situation not found");
        }
        var question = await _context.Questions.FindAsync(questionId);
        if (question == null || question.IsDeleted)
        {
            _logger.LogWarning("Question not found");
            return new ServiceResponse("Question not found");
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
            return new ServiceResponse("Error while saving changes");
        }
    }
}
