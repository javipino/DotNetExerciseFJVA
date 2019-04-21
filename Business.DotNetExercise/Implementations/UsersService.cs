using AutoMapper;
using Integration.DataInfraestructure;
using Shared.Domain.Tables;
using Shared.ViewModel.Users;
using System.Collections.Generic;
using System.Linq;

namespace Business.Users.Implementations
{
    class UserService : IUsersService
    {
        #region Attributes

        protected IRepository<User> _userRepo;
        protected IUnitOfWork _unitOfWork;
        protected IMapper _mapper;

        #endregion

        #region Constructors

        public UserService(IRepository<User> userRepo,
                           IUnitOfWork unitOfWork,
                           IMapper mapper)
        {
            _userRepo = userRepo;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        #endregion

        /// <summary>
        /// Get All Users
        /// </summary>
        /// <returns>List of fech Users</returns>
        public List<UserModel> Get()
        {
            var result = _userRepo.Get().GetAwaiter().GetResult().ToList();
            return _mapper.Map<List<UserModel>>(result);
        }

        /// <summary>
        /// Get User Model by Pk
        /// </summary>
        /// <param name="id">User Identifier</param>
        /// <returns>User model or empty if not found</returns>
        public UserModel Get(int id)
        {
            var result = _userRepo.GetByPK(new User() { Id = id}).GetAwaiter().GetResult();
            return _mapper.Map<UserModel>(result);
        }

        /// <summary>
        /// Insert Model in Db
        /// </summary>
        /// <param name="value">User Model</param>
        /// <returns>returner Id from insert Model</returns>
        public int Post(UserModel value)
        {
            var result = _userRepo.Insert(_mapper.Map<User>(value));
            if (result.Id != 0)
            {
                _unitOfWork.SaveChanges();
                return result.Id;
            }
            return result.Id;
        }

        /// <summary>
        /// Update Db Element
        /// </summary>
        /// <param name="id">Model Identifier</param>
        /// <param name="value">Model value</param>
        /// <returns>number of modifications done</returns>
        public int Put(int id, UserModel value)
        {
            int result = 0;
            var user = _userRepo.GetByPK(new User() { Id = id }).GetAwaiter().GetResult();
            if (user != null)
            {
                value.Id = id;
                result = _userRepo.Update(_mapper.Map<User>(value)).GetAwaiter().GetResult();
                if (result != 0)
                {
                    _unitOfWork.SaveChanges();
                }
            }
            return result;
        }

        /// <summary>
        /// Physically
        /// </summary>
        /// <param name="id">Element identifaction</param>
        /// <returns>number of modifications done</returns>
        public int Delete(int id)
        {
            int result = 0;
            var user = _userRepo.GetByPK(new User() { Id = id }).GetAwaiter().GetResult();
            if (user != null)
            {
                result = _userRepo.DeletePhysical(new User() { Id = id }).GetAwaiter().GetResult();
                if (result != 0)
                {
                    _unitOfWork.SaveChanges();
                }
            }
            return result;
        }
    }
}
