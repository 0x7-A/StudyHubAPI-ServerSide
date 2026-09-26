using StudyHubAPI.Models.DTOs.Damage;
using StudyHubAPI.Models.DTOs.WorkspaceImages;
using StudyHubAPI.Models.Entities;
using StudyHubAPI.Repositories;
using StudyHubAPI.Utils;

namespace StudyHubAPI.Services
{
    public class DamagesService
    {
        private readonly DamagesRepository _damagesRepository;
        private readonly IConfiguration _configuration;


        public DamagesService(DamagesRepository damagesRepository, IConfiguration configuration)
        {
            _damagesRepository = damagesRepository;
            _configuration = configuration;
        }


        public async Task<ServiceResult<int>> AddDamagedRecord(CreateDamageDto dto)
        {

            string? fullFilePath = null;

            if (dto.File != null)
            {

                if (!await ImageValidator.IsValidImageAsync(dto.File))
                {
                    return ServiceResult<int>.Failure(ResultType.BadRequest, "Invalid image file.");
                }

                var ext = Path.GetExtension(dto.File.FileName).ToLowerInvariant();


                if (await ImageValidator.IsValidImageAsync(dto.File))
                {
                    return ServiceResult<int>.Failure(ResultType.BadRequest, "Invalid image file.");
                }

                var fileName = $"{Guid.NewGuid()}{ext}";

                var folderPath = _configuration["FileStorage:WorkspaceImagesPath"] ?? "C:\\WorkspaceImages";

                Directory.CreateDirectory(folderPath);
                fullFilePath = Path.Combine(folderPath, fileName);

                using (var stream = new FileStream(fullFilePath, FileMode.Create))
                {
                    await dto.File.CopyToAsync(stream);
                }
            }


            var damaged = new Damage
            {
                PaymentId = dto.PaymentId,
                Notes = dto.Notes,
                ImagePath = fullFilePath
            };


            if (await _damagesRepository.AddDamagedRecord(damaged) > 0)
            {
                return ServiceResult<int>.Success(damaged.DamageId, ResultType.Created);
            }

            return ServiceResult<int>.Failure(ResultType.Failure, "Failed to add damaged record.");
        }

        public async Task<ServiceResult<DamageSummaryDto?>> GetDamageRecord(int paymentID, string baseURL)
        {
         
            var damageRecord = await _damagesRepository.GetDamageRecord(paymentID);
            if (damageRecord == null)
            {
                return ServiceResult<DamageSummaryDto?>.Failure(ResultType.NotFound, "Damage record not found.");
            }

            damageRecord.ImageUrl = $"{baseURL}/api/WorkspaceImages/{paymentID}/file";
            return ServiceResult<DamageSummaryDto?>.Success(damageRecord, ResultType.Ok);
        }

        public async Task<ServiceResult<ImageFileStreamDto?>> GetImageFileAsync(int PaymentID)
        {
            var Path = await _damagesRepository.GetDamageImagePathByPaymentId(PaymentID);
            if (Path == null)
                return ServiceResult<ImageFileStreamDto?>.Failure(ResultType.NotFound, "Image not found.");


            if (!File.Exists(Path))
                return ServiceResult<ImageFileStreamDto?>.Failure(ResultType.NotFound, "Image file not found.");

            var ext = System.IO.Path.GetExtension(Path).ToLowerInvariant();
            var contentType = ext switch
            {
                ".jpg" or ".jpeg" => "image/jpeg",
                ".png" => "image/png",
                ".webp" => "image/webp",
                _ => "application/octet-stream"
            };

            return ServiceResult<ImageFileStreamDto?>.Success(new ImageFileStreamDto
            { FilePath = Path, ContentType = contentType }, ResultType.Ok);
        }

    }
}
