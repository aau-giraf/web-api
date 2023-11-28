using System;
using System.Collections.Generic;
using System.Text;
using GirafRepositories.Interfaces;
using GirafAPI.Controllers;
using GirafServices.User;
using Moq;
using Microsoft.Extensions.Logging;

namespace Giraf.UnitTest.Mocks
{
    class MockedWeekController : WeekController
    {
        public Mock<IUserService> Giraf { get; set; }
        public Mock<ILoggerFactory> LoggerFactory { get; set; }
        public Mock<IWeekRepository> WeekRepository { get; set; }
        public Mock<ITimerRepository> TimerRepository { get; set; }
        public Mock<IPictogramRepository> PictogramRepository { get; set; }
        public Mock<IWeekdayRepository> WeekdayRepository { get; set; }
        public MockedWeekController() : this(new Mock<IUserService>(),
                                            new Mock<ILoggerFactory>(),
                                            new Mock<IWeekRepository>(),
                                            new Mock<ITimerRepository>(),
                                            new Mock<IPictogramRepository>(),
                                            new Mock<IWeekdayRepository>())
        {
        }



        public MockedWeekController(Mock<IUserService> giraf,
                                    Mock<ILoggerFactory> loggerFactory,
                                    Mock<IWeekRepository> weekRepository,
                                    Mock<ITimerRepository> timerRepository,
                                    Mock<IPictogramRepository> pictogramRepository,
                                    Mock<IWeekdayRepository> weekdayRepository) : base(
                                        giraf.Object,
                                        loggerFactory.Object,
                                        weekRepository.Object,
                                        timerRepository.Object,
                                        pictogramRepository.Object,
                                        weekdayRepository.Object)
        {
            Giraf = giraf;
            LoggerFactory = loggerFactory;
            WeekRepository = weekRepository;
            TimerRepository = timerRepository;
            PictogramRepository = pictogramRepository;
            WeekdayRepository = weekdayRepository;
        }
    }
}
