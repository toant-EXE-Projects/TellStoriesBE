using AutoMapper;
using Mscc.GenerativeAI;
using StoryTeller.Data.Entities;
using StoryTeller.Data.Models;
using StoryTeller.Services.Implementations;
using StoryTeller.Services.Models.AI.OptionsModel;
using StoryTeller.Services.Models.DTO;
using StoryTeller.Services.Models.RequestModel;
using StoryTeller.Services.Models.ResponseModel;
using VNPAY.NET.Models;


namespace StoryTeller.Services.AutoMapperMapping
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<RegisterRequest, ApplicationUser>().ReverseMap();
            CreateMap<ApplicationUser, UserDTO>().ReverseMap();
            CreateMap<ApplicationUser, UserMinimalDTO>().ReverseMap();
            CreateMap<ApplicationUser, UserCreateRequest>().ReverseMap();
            CreateMap<ApplicationUser, UserUpdateRequest>().ReverseMap();
            CreateMap<ApplicationUser, ProfileUpdateRequest>().ReverseMap();
            CreateMap<UserUpdateRequest, ProfileUpdateRequest>().ReverseMap();
            CreateMap<PaginatedResult<ApplicationUser>, PaginatedResult<UserDTO>>().ReverseMap();

            CreateMap<Story, StoryCreateRequest>().ReverseMap();
            CreateMap<Story, StoryDTO>()
                .ForMember(dest => dest.Tags,
                    opt => opt.MapFrom(src => src.StoryTags
                        .Where(st => st.Tag != null)
                        .Select(st => st.Tag) ?? new List<Tag>()))
                .ForMember(dest => dest.TagNames,
                    opt => opt.MapFrom(src => src.StoryTags
                        .Where(st => st.Tag != null)
                        .Select(st => st.Tag.Name)
                        .ToList() ?? new List<string>()))
                .ForMember(dest => dest.Content,
                    opt => opt.MapFrom(src =>
                        string.Join("\n\n", src.Panels
                            .OrderBy(p => p.PanelNumber)
                            .Select(p => p.Content)
                            .Where(content => !string.IsNullOrWhiteSpace(content))
                        )
                    )
                )
                .ReverseMap();
            CreateMap<PaginatedResult<Story>, PaginatedResult<StoryDTO>>().ReverseMap();

            CreateMap<Story, StoryMeta>().ReverseMap();
            CreateMap<Story, StoryUpdateRequest>().ReverseMap();
            CreateMap<Story, StoryUpdateMetaRequest>().ReverseMap();
            CreateMap<StoryPanel, StoryPanelCreateRequest>().ReverseMap();
            CreateMap<Tag, TagDTO>().ReverseMap();
            CreateMap<AddTagsRequest, AddTagsToStoryRequest>().ReverseMap();

            CreateMap<ActivityLog, ActivityLogDTO>().ReverseMap();
            CreateMap<ActivityLog, ActivityLogCreateRequest>().ReverseMap();

            CreateMap<Subscription, SubscriptionDTO>().ReverseMap();
            CreateMap<Subscription, SubscriptionCreateRequest>().ReverseMap();
            CreateMap<Subscription, SubscriptionUpdateRequest>().ReverseMap();

            CreateMap<UserLibrary, UserLibraryCreateRequest>().ReverseMap();
            CreateMap<UserLibrary, UserLibraryDTO>().ReverseMap();
            CreateMap<UserLibrary, UserLibraryMinimalDTO>().ReverseMap();
            CreateMap<UserLibrary, UserLibraryEditRequest>().ReverseMap();
            CreateMap<UserLibraryItem, UserLibraryItemCreateRequest>().ReverseMap();
            CreateMap<UserLibraryItem, UserLibraryItemDTO>().ReverseMap();


            CreateMap<GenerationConfig, ChatGenerationOptions>().ReverseMap();

            CreateMap<Comment, CommentDTO>().ReverseMap();
            CreateMap<Comment, CommentCreateRequest>().ReverseMap();
            CreateMap<Comment, CommentMeta>().ReverseMap();
            CreateMap<Comment, CommentThreadDTO>().ReverseMap();
            CreateMap<PaginatedResult<Comment>, PaginatedResult<CommentDTO>>().ReverseMap();

            CreateMap<UserSubscription, SubscriptionDetailResponse>()                
                .ForMember(dest => dest.User,
                    opt => opt.MapFrom(src => src.User))
                .ForMember(dest => dest.UserId,
                    opt => opt.MapFrom(src => src.UserId))
                .ForMember(dest => dest.Plan,
                    opt => opt.MapFrom(src => src.Subscription.Name))
                .ForMember(dest => dest.Price,
                    opt => opt.MapFrom(src => src.Subscription.Price))
                .ForMember(dest => dest.Duration,
                    opt => opt.MapFrom(src => src.Subscription.DurationDays))
                .ForMember(dest => dest.SubscribedOn,
                    opt => opt.MapFrom(src => src.StartDate))
                .ForMember(dest => dest.ExpiriesOn,
                    opt => opt.MapFrom(src => src.EndDate))
                .ReverseMap();
            CreateMap<UserSubscription, UserSubscriptionDTO>().ReverseMap();

            CreateMap<StoryPublishRequest, StoryPublishRequestDTO>().ReverseMap();
            CreateMap<StoryPublishRequest, StoryPublishCreateRequest>().ReverseMap();
            CreateMap<StoryPublishRequest, StoryPublishReviewRequest>().ReverseMap();
            CreateMap<PaginatedResult<StoryPublishRequest>, PaginatedResult<StoryPublishRequestDTO>>().ReverseMap();

            CreateMap<IssueReport, IssueReportDTO>().ReverseMap();
            CreateMap<IssueReport, IssueReportCreateRequest>().ReverseMap();
            CreateMap<PaginatedResult<IssueReport>, PaginatedResult<IssueReportDTO>>().ReverseMap();

            CreateMap<CensoredWord, CensoredWordDTO>().ReverseMap();

            CreateMap<PaymentResult, PaymentResultDTO>().ReverseMap();

            CreateMap<Notification, NotificationDTO>().ReverseMap();
            CreateMap<Notification, NotificationSendRequest>().ReverseMap();
            CreateMap<PaginatedResult<Notification>, PaginatedResult<NotificationDTO>>().ReverseMap();

            CreateMap<UserWallet, UserWalletDTO>().ReverseMap();

            CreateMap<BillingRecord, BillingRecordDTO>().ReverseMap();

            CreateMap<SystemConfiguration, SystemConfigurationDTO>().ReverseMap();

            CreateMap<UserWalletTransaction, UserWalletTransactionDTO>().ReverseMap();
            CreateMap<PaginatedResult<UserWalletTransaction>, PaginatedResult<UserWalletTransactionDTO>>().ReverseMap();
            CreateMap<UserWalletTransactionRequest, WalletTransactionRequest>().ReverseMap();
        }
    }
}
