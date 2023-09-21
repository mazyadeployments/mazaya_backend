using MMA.WebApi.Shared.Constants;
using MMA.WebApi.Shared.Interfaces.ApplicationUsers;
using MMA.WebApi.Shared.Interfaces.Banner;
using MMA.WebApi.Shared.Interfaces.Categories;
using MMA.WebApi.Shared.Interfaces.Collection;
using MMA.WebApi.Shared.Interfaces.Companies;
using MMA.WebApi.Shared.Interfaces.Dashboard;
using MMA.WebApi.Shared.Interfaces.OfferRating;
using MMA.WebApi.Shared.Interfaces.Offers;
using MMA.WebApi.Shared.Interfaces.Roles;
using MMA.WebApi.Shared.Interfaces.Tag;
using MMA.WebApi.Shared.Models.Dashboard;
using System.Collections.Generic;
using System.Threading.Tasks;
using static MMA.WebApi.Shared.Enums.Declares;
namespace MMA.WebApi.Core.Services
{
    public class DashboardService : IDashboardService
    {
        private readonly IRoleService _roleService;
        private readonly IOfferRepository _offerRepository;
        private readonly ICompanyRepository _companyRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly ICollectionRepository _collectionRepository;
        private readonly IBannerRepository _bannerRepository;
        private readonly IApplicationUsersRepository _applicationUsersRepository;
        private readonly IOfferRatingRepository _offerRatingRepository;
        private readonly ITagRepository _tagRepository;
        private readonly IRoadshowRepository _roadshowRepository;

        public DashboardService(IRoleService roleService,
                                IOfferRepository offerRepository,
                                ICompanyRepository companyRepository,
                                ICategoryRepository categoryRepository,
                                ICollectionRepository collectionRepository,
                                IBannerRepository bannerRepository,
                                IApplicationUsersRepository applicationUsersRepository,
                                IOfferRatingRepository offerRatingRepository,
                                ITagRepository tagRepository,
                                IRoadshowRepository roadshowRepository)
        {
            _roleService = roleService;
            _offerRepository = offerRepository;
            _companyRepository = companyRepository;
            _categoryRepository = categoryRepository;
            _collectionRepository = collectionRepository;
            _bannerRepository = bannerRepository;
            _applicationUsersRepository = applicationUsersRepository;
            _offerRatingRepository = offerRatingRepository;
            _tagRepository = tagRepository;
            _roadshowRepository = roadshowRepository;
        }

        public async Task<IEnumerable<DashboardModel>> GetDashboardItems(string userId)
        {
            var roles = await _roleService.GetUserRoles(userId);
            List<DashboardModel> dashboardModels = new List<DashboardModel>();
            if (roles.Contains(Roles.Supplier))
            {
                var allOffers = await _offerRepository.GetAllOffersCount(userId, roles);
                var myOffers = await _offerRepository.GetMyOffersCount(userId, roles);

                var myRoadshows = await _roadshowRepository.GetRoadshowCount(userId, roles);

                DashboardModel allOffersDashboardModel = new DashboardModel { Title = DashboardConstants.AllOffers, Count = allOffers };
                DashboardModel myOffersDashboardModel = new DashboardModel { Title = DashboardConstants.MyOffers, Count = myOffers };

                DashboardModel myRoadshowsModel = new DashboardModel { Title = DashboardConstants.MyRoadshows, Count = myRoadshows };

                dashboardModels.Add(allOffersDashboardModel);
                dashboardModels.Add(myOffersDashboardModel);
                dashboardModels.Add(myRoadshowsModel);
            }
            else if (roles.Contains(Roles.SupplierAdmin))
            {
                var allOffers = await _offerRepository.GetAllOffersCount(userId, roles);
                var myOffers = await _offerRepository.GetMyOffersCount(userId, roles);

                // supplierAdminId == userId
                var allFocalPoints = await _companyRepository.GetMyFocalPointsCount(userId);

                var myRoadshows = await _roadshowRepository.GetRoadshowCount(userId, roles);

                DashboardModel allOffersDashboardModel = new DashboardModel { Title = DashboardConstants.AllOffers, Count = allOffers };
                DashboardModel myOffersDashboardModel = new DashboardModel { Title = DashboardConstants.MyOffers, Count = myOffers };

                DashboardModel allFocalPointsDashboardModel = new DashboardModel { Title = DashboardConstants.AllFocalPoints, Count = allFocalPoints };

                DashboardModel myRoadshowsModel = new DashboardModel { Title = DashboardConstants.MyRoadshows, Count = myRoadshows };

                dashboardModels.Add(allOffersDashboardModel);
                dashboardModels.Add(myOffersDashboardModel);

                dashboardModels.Add(allFocalPointsDashboardModel);
                dashboardModels.Add(myRoadshowsModel);
            }
            else if (roles.Contains(Roles.Reviewer))
            {
                var allOffers = await _offerRepository.GetAllOffersCount(userId, roles);
                var assignedOffers = await _offerRepository.GetAssignedOffersCountForReviewer();
                var myOffers = await _offerRepository.GetMyOffersCount(userId, roles);


                DashboardModel allOffersDashboardModel = new DashboardModel { Title = DashboardConstants.AllOffers, Count = allOffers };
                DashboardModel assignedOffersDashboardModel = new DashboardModel { Title = DashboardConstants.AssignedOffers, Count = assignedOffers };
                DashboardModel myOffersDashboardModel = new DashboardModel { Title = DashboardConstants.MyOffers, Count = myOffers };

                dashboardModels.Add(allOffersDashboardModel);
                dashboardModels.Add(assignedOffersDashboardModel);
                dashboardModels.Add(myOffersDashboardModel);
            }
            else if (roles.Contains(Roles.AdnocCoordinator) || roles.Contains(Roles.Admin))
            {
                //offers
                var allOffers = await _offerRepository.GetAllOffersCount(userId, roles);
                var assignedOffers = await _offerRepository.GetAssignedOffersCountForAdmin();
                var myOffers = await _offerRepository.GetMyOffersCount(userId, roles);

                //suppliers
                var allSuppliers = await _companyRepository.GetCompaniesInStatusCount(SupplierStatus.Approved.ToString());
                var pendingSuppliers = await _companyRepository.GetCompaniesInStatusCount(SupplierStatus.PendingApproval.ToString());

                var allCategories = await _categoryRepository.GetCategoriesCount();

                var allCollections = await _collectionRepository.GetCollectionsCount();

                //administration
                var allBanners = await _bannerRepository.GetBannerCount();

                string[] adnocRoles = { Roles.Reviewer.ToString(), Roles.AdnocCoordinator.ToString(), Roles.Admin.ToString() };
                var allAdnocUsers = await _applicationUsersRepository.GetUserInRoleCount(adnocRoles);

                string[] buyerRoles = { Roles.Buyer.ToString() };
                var allBuyers = await _applicationUsersRepository.GetUserInRoleCount(buyerRoles);

                var allOfferRatings = await _offerRatingRepository.GetCommentsCount();
                var allTags = await _tagRepository.GetTagsCount();

                DashboardModel allOffersDashboardModel = new DashboardModel { Title = DashboardConstants.AllOffers, Count = allOffers };
                DashboardModel assignedOffersDashboardModel = new DashboardModel { Title = DashboardConstants.AssignedOffers, Count = assignedOffers };
                DashboardModel myOffersDashboardModel = new DashboardModel { Title = DashboardConstants.MyOffers, Count = myOffers };

                DashboardModel allSuppliersDashboardModel = new DashboardModel { Title = DashboardConstants.AllSupliers, Count = allSuppliers };
                DashboardModel pendingSuppliersDashboardModel = new DashboardModel { Title = DashboardConstants.PendingSuppliers, Count = pendingSuppliers };

                DashboardModel allCategoriesDashboardModel = new DashboardModel { Title = DashboardConstants.AllCategories, Count = allCategories };

                DashboardModel allCollectionsDashboardModel = new DashboardModel { Title = DashboardConstants.AllCollections, Count = allCollections };

                DashboardModel allBannersDashboardModel = new DashboardModel { Title = DashboardConstants.AllBanners, Count = allBanners };
                DashboardModel allAdnocUsersDashboardModel = new DashboardModel { Title = DashboardConstants.AllAdnocUsers, Count = allAdnocUsers };
                DashboardModel allBuyersDashboardModel = new DashboardModel { Title = DashboardConstants.AllBuyers, Count = allBuyers };
                DashboardModel allOfferRatingsDashboardModel = new DashboardModel { Title = DashboardConstants.AllOfferRating, Count = allOfferRatings };
                DashboardModel allTagsDashboardModel = new DashboardModel { Title = DashboardConstants.AllTags, Count = allTags };



                dashboardModels.Add(allOffersDashboardModel);
                dashboardModels.Add(assignedOffersDashboardModel);
                dashboardModels.Add(myOffersDashboardModel);

                dashboardModels.Add(allSuppliersDashboardModel);
                dashboardModels.Add(pendingSuppliersDashboardModel);

                dashboardModels.Add(allCategoriesDashboardModel);

                dashboardModels.Add(allCollectionsDashboardModel);

                dashboardModels.Add(allBannersDashboardModel);
                dashboardModels.Add(allAdnocUsersDashboardModel);
                dashboardModels.Add(allBuyersDashboardModel);
                dashboardModels.Add(allOfferRatingsDashboardModel);
                dashboardModels.Add(allTagsDashboardModel);

            }
            return dashboardModels;
        }
    }
}
