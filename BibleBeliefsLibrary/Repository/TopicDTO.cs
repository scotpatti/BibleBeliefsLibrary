namespace BibleBeliefs.Repository
{
    public class TopicDTO : BaseDTO
    {
        private long _Id;
        private string _Topic;

        public TopicDTO()
        {
            _Topic = "";
        }

        public long Id 
        { 
            get { return _Id; }
            set { SetField<long>(ref _Id, value); }
        }

        public string Topic
        {
            get { return _Topic; }
            set { SetField<string>(ref _Topic, value); }
        }

        public override string ToString()
        {
            return _Topic;
        }
    }
}
