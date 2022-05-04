namespace BibleBeliefs.Repository
{
    public class BeliefDTO : BaseDTO
    {
        private long _Id;
        private string _Belief;
        private long _TopicId;

        public BeliefDTO()
        {
            _Belief = "";
        }

        public long Id
        {
            get { return _Id; }
            set { SetField<long>(ref _Id, value); }
        }

        public string Belief
        {
            get { return _Belief; }
            set { SetField<string>(ref _Belief, value); }
        }

        public long TopicId
        {
            get { return _TopicId; }
            set { SetField<long>(ref _TopicId, value); }
        }

        public override string ToString()
        {
            return _Belief;
        }
    }
}
