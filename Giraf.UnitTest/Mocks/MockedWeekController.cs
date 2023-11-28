using System;
using System.Collections.Generic;
using System.Text;
using GirafRepositories.Interfaces;
using GirafAPI.Controllers;
using GirafServices.User;
using GirafServices.WeekPlanner;
using Moq;
using Microsoft.Extensions.Logging;

namespace Giraf.UnitTest.Mocks
{
    class MockedWeekController : WeekController
    {
        public Mock<IWeekRepository> WeekRepository { get; set; }
        public Mock<ITimerRepository> TimerRepository { get; set; }
        public Mock<IPictogramRepository> PictogramRepository { get; set; }

        public Mock<IWeekService> WeekService { get; set; }
        public Mock<IWeekdayRepository> WeekdayRepository { get; set; }
        public MockedWeekController() : this(new Mock<IWeekRepository>(),
                                            new Mock<ITimerRepository>(),
                                            new Mock<IPictogramRepository>(),
                                            new Mock<IWeekdayRepository>(),
                                            new Mock<IWeekService>())
        {
        }



        public MockedWeekController(Mock<IWeekRepository> weekRepository,
                                    Mock<ITimerRepository> timerRepository,
                                    Mock<IPictogramRepository> pictogramRepository,
                                    Mock<IWeekdayRepository> weekdayRepository,
                                    Mock<IWeekService> weekService) : base(
                                        weekRepository.Object,
                                        timerRepository.Object,
                                        pictogramRepository.Object,
                                        weekdayRepository.Object,
                                        weekService.Object)
        {
            WeekRepository = weekRepository;
            TimerRepository = timerRepository;
            PictogramRepository = pictogramRepository;
            WeekdayRepository = weekdayRepository;
            WeekService = weekService;

        }
    }
}
