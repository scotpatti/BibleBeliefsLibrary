using Xunit;
using BibleBeliefs.Repository;
using System.Linq;
using System;

[assembly: CollectionBehavior(DisableTestParallelization = true)]

namespace TestBibleBeliefsLibrary
{
    public class BibleBeliefsTopicRepoTests
    {
        [Fact]
        public void TestTopics()
        {
            var repo = new BibleBeliefsRepository();
            //Test Create Topic
            TopicDTO topicDTO = new TopicDTO() { Topic = "TrashTalk" };
            var added = repo.CreateTopic(topicDTO);
            Assert.Equal(topicDTO.Topic, added.Topic);
            //Test Read by text
            var result = repo.GetTopicByText(topicDTO.Topic);
            Assert.True(result.Topic.Equals(topicDTO.Topic));
            Assert.True(result.Id.Equals(added.Id));
            //Test Read All
            var list = repo.GetTopics();
            Assert.NotNull(list);
            Assert.True(list.Count > 0);
            var val = list.Where(x => x.Id == result.Id).FirstOrDefault();
            Assert.NotNull(val);
            //Test Update
            topicDTO.Topic = "Raunchy Commedy";
            topicDTO.Id = result.Id;
            Assert.True(repo.UpdateTopic(topicDTO));
            result = repo.GetTopicByText(topicDTO.Topic);
            Assert.True(result.Topic == topicDTO.Topic);
            Assert.True(result.Id == result.Id);
            //Test Delete
            Assert.True(repo.DeleteTopic(topicDTO));
            repo.Close();
        }
    }

    public class BibleBeliefsRepoTests
    {

        [Fact]
        public void TestBeliefs()
        {
            var repo = new BibleBeliefsRepository();
            //Test Create Belief
            var belief = new BeliefDTO() { Belief = "Test Belief", TopicId = 1 };
            var result = repo.CreateBelief(belief);
            Assert.NotNull(result);
            if (result != null)
            {
                Assert.Equal(belief.Belief, result.Belief);
                Assert.Equal(belief.TopicId, result.TopicId);
            }
            else throw new Exception("Beliefs created incorrectly!");

            //Test Read Belief given topic
            var list = repo.GetBeliefs(1);
            Assert.True(list.Count > 0);

            //Test Update Belief
            result.Belief = "Testing Update Beleif";
            Assert.True(repo.UpdateBelief(result));

            //Test Delete Belief give Id.
            Assert.True(repo.DeleteBelief(result.Id));
            repo.Close();
        }
    }
    
    public class BibleBeliefsVersesTests 
    { 

        [Fact]
        public void TestVerses()
        {
            var repo = new BibleBeliefsRepository();
            //Test Create
            var verse = new VerseDTO() { VerseText = "Revelation 1:1", BeliefId = 1, Book = 65, Chapter = 0, VerseStart = 0, VerseEnd = 0 };
            var result = repo.CreateVerse(verse);
            Assert.NotNull(result);
            Assert.True(verse.Equals(result));

            //Test Read
            var list = repo.GetVerses(1);
            Assert.NotNull(list);
            Assert.True(list.Count > 0);

            //Test Update
            verse.Id = result.Id;
            verse.VerseEnd = 1;
            verse.VerseText = "Revelation 1:1-2";
            Assert.True(repo.UpdateVerse(verse));

            //Test Delete
            Assert.True(repo.DeleteVerse(verse.Id));
            repo.Close();
        }
    }
}