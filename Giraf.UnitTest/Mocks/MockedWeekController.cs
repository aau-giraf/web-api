using System;
using System.Collections.Generic;
using System.Text;
using GirafRest.Controllers;
using Moq;
using GirafRest.IRepositories;
using GirafRest.Interfaces;
using Microsoft.Extensions.Logging;
using GirafRest.Shared;

namespace Giraf.UnitTest.Mocks
{
    class MockedWeekController : WeekController
    {
        public Mock<IGirafService> Giraf { get; set; }
        public Mock<ILoggerFactory> LoggerFactory { get; set; }
        public Mock<IWeekRepository> WeekRepository { get; set; }
        public Mock<ITimerRepository> TimerRepository { get; set; }
        public Mock<IPictogramRepository> PictogramRepository { get; set; }
        public Mock<IWeekdayRepository> WeekdayRepository { get; set; }
        public MockedWeekController() : this(new Mock<IGirafService>(),
                                            new Mock<ILoggerFactory>(),
                                            new Mock<IWeekRepository>(),
                                            new Mock<ITimerRepository>(),
                                            new Mock<IPictogramRepository>(),
                                            new Mock<IWeekdayRepository>())
        {
        }



        public MockedWeekController(Mock<IGirafService> giraf,
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
