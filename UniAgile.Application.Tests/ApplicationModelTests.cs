using System.Collections.Specialized;
using Moq;
using UniAgile.Game;
using Xunit;

namespace UniAgile.Application.Tests.ApplicationModelTests
{
    public class Unit
    {
        [Fact]
        public void Application_model_can_store_repositories()
        {
            var repo = new Repository<int>();

            var applicationModel = new ApplicationModel(new IRepository[]
            {
                repo
            });

            Assert.Equal(applicationModel.GetRepository<int>(), repo);
        }

        [Fact]
        public void Application_model_can_have_delegates_listen_for_data_changes()
        {
            var repo = new Repository<int>();

            var applicationModel = new ApplicationModel(new IRepository[]
            {
                repo
            });

            var mockListener = new Mock<NotifyCollectionChangedEventHandler>();
            var testId = "test";

            applicationModel[testId].CollectionChanged += mockListener.Object;

            applicationModel.GetRepository<int>()[testId] = 666;
            applicationModel.NotifyChanges();

            mockListener.Verify(d => d.Invoke(It.IsAny<object>(), It.IsAny<NotifyCollectionChangedEventArgs>()),
                                Times.Once);
        }

        [Fact]
        public void Application_model_can_notify_listeners_of_data_changes_which_have_not_been_notified()
        {
            var repo = new Repository<int>();

            var applicationModel = new ApplicationModel(new IRepository[]
            {
                repo
            });

            var mockListener = new Mock<NotifyCollectionChangedEventHandler>();
            var testId = "test";

            applicationModel[testId].CollectionChanged += mockListener.Object;

            applicationModel.GetRepository<int>()[testId] = 666;
            applicationModel.NotifyChanges();
            applicationModel.NotifyChanges();

            mockListener.Verify(d => d.Invoke(It.IsAny<object>(), It.IsAny<NotifyCollectionChangedEventArgs>()),
                                Times.Once);
        }
    }
}