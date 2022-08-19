using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using Tasker.Core.Aggregates.TaskAggregate;
using Tasker.Core.Constants;

namespace Tasker.Tests.Core.TaskAggregate
{
    [TestFixture]
    public class TaskHistoryItemTests
    {
        [Test]
        //[TestCaseSource("ConstructorInputs")]
        public void GetHashCode_ShouldReturnCorrectValue(/*DateTime timeStamp, int workerId, TaskCompletionStatus taskCompletionStatus*/)
        {
            var random = new Random().Next(1, 1000);
            var timestamp = DateTime.Now.AddDays(random).AddMilliseconds(random);
            var workerId = random;
            var status = (TaskCompletionStatus)(random % 2);
            var expectedHashCode = typeof(TaskHistoryItem).GetHashCode() + timestamp.GetHashCode() + workerId.GetHashCode() + status.GetHashCode();
            var sut = new TaskHistoryItem(timestamp, workerId, status);

            Assert.AreEqual(expectedHashCode, sut.GetHashCode());
        }

        [Test]
        public void Equals_ShouldReturnTrueIfAllMembersAreEqual()
        {
            var timestamp = DateTime.Now.AddDays(34).AddHours(14).AddSeconds(1000);
            var sut = new TaskHistoryItem(timestamp, 123, TaskCompletionStatus.Done);

            Assert.IsTrue(sut.Equals(new TaskHistoryItem(timestamp, 123, TaskCompletionStatus.Done)));
            Assert.IsFalse(sut.Equals(new TaskHistoryItem(timestamp.AddDays(-1), 123, TaskCompletionStatus.Done)));
            Assert.IsFalse(sut.Equals(new TaskHistoryItem(timestamp, 12677, TaskCompletionStatus.Done)));
            Assert.IsFalse(sut.Equals(new TaskHistoryItem(timestamp, 123, TaskCompletionStatus.Skipped)));
        }

        //public static object[] ConstructorInputs = 
        //{ 
        //    new object[] { DateTime.Now, 1, TaskCompletionStatus.Skipped },
        //    new object[] { DateTime.Now.AddDays(12).AddMilliseconds(10000), new Random().Next(), TaskCompletionStatus.Done },
        //    new object[] { DateTime.Now.AddDays(-1), int.MaxValue, TaskCompletionStatus.Skipped }
        //};
    }
}
