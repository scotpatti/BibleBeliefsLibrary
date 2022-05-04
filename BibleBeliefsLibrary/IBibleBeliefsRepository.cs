using BibleBeliefs.Repository;
using System.ComponentModel;

namespace BibleBeliefsLibrary
{
    public interface IBibleBeliefsRepository
    {
        public void Close();

        public TopicDTO CreateTopic(TopicDTO topic);
        public TopicDTO GetTopicByText(string topic);
        public BindingList<TopicDTO> GetTopics();
        public bool UpdateTopic(TopicDTO topic);
        public bool DeleteTopic(TopicDTO topic);

        public BeliefDTO? CreateBelief(BeliefDTO belief);
        public BindingList<BeliefDTO> GetBeliefs(long topicId);
        public bool UpdateBelief(BeliefDTO belief);
        public bool DeleteBelief(long beliefId);

        public VerseDTO CreateVerse(VerseDTO verse);
        public BindingList<VerseDTO> GetVerses(long beliefId);
        public bool UpdateVerse(VerseDTO verse);
        public bool DeleteVerse(long verseId);
    }
}
