using GirafRest.Controllers;
using GirafRest.Models.DTOs.AccountDTOs;
using GirafRest.Test.Mocks;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;
using Xunit.Abstractions;
using static GirafRest.Test.UnitTestExtensions;
using GirafRest.Services;
using GirafRest.Models.DTOs;
using GirafRest.Models.Responses;
using Microsoft.Extensions.Options;
using System.Linq;
using System;
using Microsoft.AspNetCore.Http;
using Castle.Core.Logging;
using GirafRest.Interfaces;
using GirafRest.IRepositories;

namespace GirafRest.Test.CHANGETHISNAMESPACE
{
     public class MockWeekController : WeekController
        {
           
            
            public MockWeekController(

                Mock<IGirafService> giraf, 
                Mock<ILoggerFactory> loggerFactory, 
                Mock<IWeekRepository>weekRepository, 
                Mock<ITimerRepository> timerRepository, 
                Mock<IPictogramRepository> pictogramRepository,
                Mock<IWeekdayRepository> weekdayRepository) 
                : base(
                    giraf.Object,
                    loggerFactory.Object,
                    weekRepository.Object,
                    timerRepository.Object,
                    pictogramRepository.Object,
                    weekdayRepository.Object
                )
            {
                GirafService = giraf;
                LoggerFactory = loggerFactory;
                Configuration = configuration;
                UserRepository = userRepository;
                DepartmentRepository = departmentRepository;
                GirafRoleRepository = girafRoleRepository;
            }
            
            
            public Mock<ILoggerFactory> LoggerFactory { get; }
            public Mock<IGirafService> GirafService { get; }
            public Mock<IOptions<JwtConfig>> Configuration { get; }
            public Mock<IGirafUserRepository> UserRepository { get; }
            public Mock<IDepartmentRepository> DepartmentRepository { get; }
            public Mock<IGirafRoleRepository> GirafRoleRepository { get; }
}