namespace BibleBeliefs.Repository
{
    public class VerseDTO : BaseDTO
    {
        private long _Id;
        private string _VerseText;
        private long _Book;
        private long _Chapter;
        private long _VerseStart;
        private long _VerseEnd;
        private long _BeliefId;

        public VerseDTO ()
        {
            _VerseText = "";
        }

        public long Id
        {
            get { return _Id; }
            set { SetField<long>(ref _Id, value); }
        }

        public string VerseText
        {
            get { return _VerseText; }
            set { SetField<string>(ref _VerseText, value); }
        }

        public long Book
        {
            get { return _Book; }
            set { SetField<long>(ref _Book, value); }
        }

        public long Chapter
        {
            get { return _Chapter; }
            set { SetField<long>(ref _Chapter, value); }
        }

        public long VerseStart
        {
            get { return _VerseStart; }
            set { SetField<long>(ref _VerseStart, value); }
        }

        public long VerseEnd
        {
            get { return _VerseEnd; }
            set { SetField<long>(ref _VerseEnd, value); }
        }

        public long BeliefId
        {
            get { return _BeliefId; }
            set { SetField<long>(ref _BeliefId, value); }
        }

        public override string ToString()
        {
            long ch = Chapter + 1;
            long vs = VerseStart + 1;
            long ve = VerseEnd + 1;
            if (VerseEnd > VerseStart)
            {
                return Books.BookArray[Book] + " " + ch + ":" + vs + "-" + ve;
            }
            else
            {
                return Books.BookArray[Book] + " " + ch + ":" + vs;
            }
        }

        public override bool Equals(object? obj)
        {
            try
            {
                if (obj is null) return false;
                var verse = obj as VerseDTO;
                if (verse == null) return false;
                return verse.VerseText == this.VerseText &&
                    verse.BeliefId == this.BeliefId &&
                    verse.Book == this.Book &&
                    verse.Chapter == this.Chapter &&
                    verse.VerseStart == this.VerseStart &&
                    verse.VerseEnd == this.VerseEnd;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public override int GetHashCode()
        {
            string hashstring = Id.ToString() + VerseText + Book.ToString() + Chapter.ToString() + VerseStart.ToString() + VerseEnd.ToString();
            return hashstring.GetHashCode();
        }
    }
}
