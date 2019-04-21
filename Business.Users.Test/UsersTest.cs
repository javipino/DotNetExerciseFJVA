using Api.Users.Controllers;
using FluentAssertions;
using Integration.DataInfraestructure;
using Microsoft.AspNetCore.Hosting.Internal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Shared.Domain.Tables;
using Shared.ViewModel.Users;
using System;
using System.Collections.Generic;
using System.IO;
using Xunit;

namespace Business.Users.Test
{
    public class UsersTest
    {
        #region Attributes

        IUsersService _service;
        IServiceProvider _provider;
        IRepository<User> _repo;
        IUnitOfWork _unitOfWork;

        #endregion

        #region Constructors
        public UsersTest()
        {
            // Setup service provider
            var serviceCollection = new ServiceCollection();
            string path = Directory.GetCurrentDirectory() + @"\..\..\..\..\Api.DotNetExercise";
            var environment = new HostingEnvironment()
            {
                ContentRootPath = path,
                EnvironmentName = "Testing"
            };
            Api.Users.Startup startup = new Api.Users.Startup(environment);
            startup.ConfigureServices(serviceCollection);
            var provider = serviceCollection.BuildServiceProvider();

            _provider = provider;

            // Retrieve services
            _service = provider.GetService<IUsersService>();
            _repo = provider.GetService<IRepository<User>>();
            _unitOfWork = provider.GetService<IUnitOfWork>();

            var initializer = new DbInitializer();
            initializer.Seed(_repo, _unitOfWork);
        }
        #endregion

        #region Get By Id  

        [Fact]
        public void GetUserById_Return_OkResult()
        {
            //Arrange  
            var controller = new UsersController(_service);
            var postId = 2;

            //Act  
            var data = controller.Get(postId);

            //Assert  
            Assert.IsType<OkObjectResult>(data);
        }

        [Fact]
        public void GetUserById_Return_NotFoundResult()
        {
            //Arrange  
            var controller = new UsersController(_service);
            var postId = 3;

            //Act  
            var data = controller.Get(postId);

            //Assert  
            Assert.IsType<NotFoundResult>(data);
        }

        [Fact]
        public void GetById_MatchResult()
        {
            //Arrange  
            var controller = new UsersController(_service);
            int postId = 1;

            //Act  
            var data = controller.Get(postId);

            //Assert  
            Assert.IsType<OkObjectResult>(data);

            var okResult = data.Should().BeOfType<OkObjectResult>().Subject;
            var post = okResult.Value.Should().BeAssignableTo<UserModel>().Subject;

            Assert.Equal("Test1", post.Name);
            Assert.Equal(DateTime.Parse("30/11/1992"), post.Birthdate);
        }

        #endregion

        #region Get All  

        [Fact]
        public void Get_Return_OkResult()
        {
            //Arrange  
            var controller = new UsersController(_service);

            //Act  
            var data = controller.Get();

            //Assert  
            Assert.IsType<OkObjectResult>(data);
        }
        
        [Fact]
        public void Get_MatchResult()
        {
            //Arrange  
            var controller = new UsersController(_service);

            //Act  
            var data = controller.Get();

            //Assert  
            Assert.IsType<OkObjectResult>(data);

            var okResult = data.Should().BeOfType<OkObjectResult>().Subject;
            var post = okResult.Value.Should().BeAssignableTo<List<UserModel>>().Subject;

            Assert.Equal("Test1", post[0].Name);
            Assert.Equal(DateTime.Parse("30/11/1992"), post[0].Birthdate);

            Assert.Equal("Test2", post[1].Name);
            Assert.Equal(DateTime.Parse("30/11/1999"), post[1].Birthdate);
        }

        #endregion

        #region Add New User 

        [Fact]
        public void Add_ValidData_Return_OkResult()
        {
            //Arrange  
            var controller = new UsersController(_service);
            var user = new UserModel() { Name = "Test3", Birthdate = DateTime.Parse("20/12/2005") };

            //Act  
            var data = controller.Post(user);

            //Assert  
            Assert.IsType<OkObjectResult>(data);
        }
        
        [Fact]
        public void Add_ValidData_MatchResult()
        {
            //Arrange  
            var controller = new UsersController(_service);
            var user = new UserModel() { Name = "Test3", Birthdate = DateTime.Parse("20/12/2005") };

            //Act  
            var data = controller.Post(user);

            //Assert  
            Assert.IsType<OkObjectResult>(data);

            var okResult = data.Should().BeOfType<OkObjectResult>().Subject; 

            Assert.Equal(3, okResult.Value);
        }

        #endregion

        #region Update Existing Blog  

        [Fact]
        public void Update_ValidData_Return_OkResult()
        {
            //Arrange  
            var controller = new UsersController(_service);
            var postId = 2;

            //Act  
            var existingPost = controller.Get(postId);
            var okResult = existingPost.Should().BeOfType<OkObjectResult>().Subject;
            var result = okResult.Value.Should().BeAssignableTo<UserModel>().Subject;

            var post = new UserModel();
            post.Name = "Test Title 2 Updated";
            post.Birthdate = DateTime.Parse("11/11/2000");

            var updatedData = controller.Put(postId, post);

            //Assert  
            Assert.IsType<OkResult>(updatedData);
        }
        
        #endregion

        #region Delete Post  

        [Fact]
        public void Delete_Post_Return_OkResult()
        {
            //Arrange  
            var controller = new UsersController(_service);
            var postId = 2;

            //Act  
            var data = controller.Delete(postId);

            //Assert  
            Assert.IsType<OkResult>(data);
        }

        [Fact]
        public void Delete_Post_Return_NotFoundResult()
        {
            //Arrange  
            var controller = new UsersController(_service);
            var postId = 5;

            //Act  
            var data = controller.Delete(postId);

            //Assert  
            Assert.IsType<NotFoundResult>(data);
        }

        #endregion
    }
}
