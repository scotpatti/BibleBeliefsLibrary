using BibleBeliefsLibrary;
using Microsoft.Data.Sqlite;
using System.ComponentModel;
using System.Data;

namespace BibleBeliefs.Repository
{
    public class BibleBeliefsRepository : IBibleBeliefsRepository
    {
        private string _dbConnString;
        private SqliteConnection? _Conn;

        public BibleBeliefsRepository(string connectionString = "BibleBeliefs.db")
        {
            _dbConnString = $"DataSource=.\\DataBase\\{connectionString};";
            _Conn = new SqliteConnection(_dbConnString);
        }

        private SqliteConnection GetConnection()
        {
            if (_Conn != null) return _Conn;
            _Conn = new SqliteConnection(_dbConnString);
            _Conn.Open();
            return _Conn;
        }

        private SqliteCommand GetNewCommand()
        {
            var conn = GetConnection();
            if (conn.State != System.Data.ConnectionState.Open) conn.Open();
            return conn.CreateCommand();
        }

        public void Close()
        {
            if (_Conn != null)
            {
                _Conn.Close();
                _Conn = null;
            }
        }

        #region Topics
        /// <summary>
        /// Given a DTO it creates the topic in the database. Don't forget to 
        /// insert your DTO into the list OR reload the topics. NOTE: This will
        /// not keep any Id associated with this topic that is handed in. 
        /// Finally, it returns the Id value added. It is possible to have 
        /// multiple, identical topics with this create functionality.
        /// </summary>
        /// <param name="topic">Topic to add to the database</param>
        /// <returns></returns>
        public TopicDTO CreateTopic(TopicDTO topic)
        {
            var comm = GetNewCommand();
            comm.CommandText = @"INSERT INTO topics(topic) VALUES(@topic)";
            comm.CommandType = CommandType.Text;
            comm.Parameters.AddWithValue("@topic", topic.Topic);
            comm.Prepare();
            var result = comm.ExecuteNonQuery();
            if (result != 1)
            {
                throw new Exception("Insert failed");
            }
            var newTopic = GetTopicByText(topic.Topic);
            return newTopic;
        }

        /// <summary>
        /// Returns the Topic object for the topic string.
        /// </summary>
        /// <param name="topic"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public TopicDTO GetTopicByText(string topic)
        {
            TopicDTO topicDto = new TopicDTO();
            SqliteCommand comm = GetNewCommand();
            comm.CommandText = @"SELECT * FROM topics WHERE topic = @topic";
            comm.CommandType = CommandType.Text;
            comm.Parameters.AddWithValue("@topic", topic);
            comm.Prepare();
            var reader = comm.ExecuteReader();
            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    topicDto.Id = reader.GetInt64(0);
                    topicDto.Topic = reader.GetString(1);
                }
            }
            else
            {
                throw new Exception($"Topic {topic} not found.");
            }
            return topicDto;
        }

        /// <summary>
        /// Returns a list of topics.
        /// </summary>
        /// <returns></returns>
        public BindingList<TopicDTO> GetTopics()
        {
            BindingList<TopicDTO> topicList = new BindingList<TopicDTO>();
            SqliteCommand comm = GetNewCommand();
            comm.CommandText = @"SELECT * FROM topics ORDER BY Topic";
            var reader = comm.ExecuteReader();
            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    TopicDTO topic = new TopicDTO();
                    topic.Id = reader.GetInt64(0);
                    topic.Topic = reader.GetString(1);
                    topicList.Add(topic);
                }
            }
            return topicList;
        }

        /// <summary>
        /// Updates the text of the topic identified by its Id.
        /// </summary>
        /// <param name="topic"></param>
        /// <returns></returns>
        public bool UpdateTopic(TopicDTO topic)
        {
            try
            {
                SqliteCommand comm = GetNewCommand();
                comm.CommandText = @"UPDATE topics SET topic = @topic WHERE _Id = @Id";
                comm.Parameters.AddWithValue("@topic", topic.Topic);
                comm.Parameters.AddWithValue("@Id", topic.Id);
                comm.Prepare();
                comm.ExecuteNonQuery();
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// Returns true if DTO was found and deleted. Returns false if the DTO
        /// was not found OR if the topic has beliefs associated with it. 
        /// </summary>
        /// <param name="topic">Topic to delete.</param>
        /// <returns>true -> success</returns>
        public bool DeleteTopic(TopicDTO topic)
        {
            try
            {
                SqliteCommand comm = GetNewCommand();
                comm.CommandText = @"DELETE FROM topics WHERE _Id = $Id";
                comm.Parameters.AddWithValue("$Id", topic.Id);
                comm.Prepare();
                comm.ExecuteNonQuery();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Check to see if this topic id is in the database already.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        private bool TopicExists(long id)
        {
            TopicDTO topicDto = new TopicDTO();
            SqliteCommand comm = GetNewCommand();
            comm.CommandText = @"SELECT * FROM topics WHERE _id = @id";
            comm.Parameters.AddWithValue("@id", id);
            comm.Prepare();
            var reader = comm.ExecuteReader();
            return reader.HasRows;    
        }

        #endregion

        #region Beliefs

        /// <summary>
        /// Creates a belief under the given topic. Topic must be specified!
        /// </summary>
        /// <param name="belief"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public BeliefDTO? CreateBelief(BeliefDTO belief)
        {
            if (!TopicExists(belief.TopicId))
            {
                throw new Exception("Can't add belief to non-existent topic.");
            }            
            SqliteCommand comm = GetNewCommand();
            comm.CommandText = @"INSERT INTO beliefs(belief, topic_id) VALUES (@belief, @topic_id)";
            comm.Parameters.AddWithValue("@belief", belief.Belief);
            comm.Parameters.AddWithValue("@topic_id", belief.TopicId);
            comm.ExecuteNonQuery();
            return GetBeliefByTextTopicId(belief.TopicId, belief.Belief);
        }

        public BindingList<BeliefDTO> GetBeliefs(long topicId)
        {
            var beliefList = new BindingList<BeliefDTO>();
            SqliteCommand comm = GetNewCommand();
            comm.CommandText = @"SELECT * FROM beliefs WHERE topic_id = $topic";
            comm.Parameters.AddWithValue("$topic", topicId);
            comm.Prepare();
            var reader = comm.ExecuteReader();
            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    BeliefDTO dto = new BeliefDTO();
                    dto.Id = reader.GetInt64(0);
                    dto.Belief = reader.GetString(1);
                    dto.TopicId = reader.GetInt64(2);
                    beliefList.Add(dto);
                }
            }
            return beliefList;
        }

        public bool UpdateBelief(BeliefDTO belief)
        {
            SqliteCommand comm = GetNewCommand();
            comm.CommandText = @"UPDATE beliefs SET belief = $belief WHERE _id = $id";
            comm.Parameters.AddWithValue("$belief", belief.Belief);
            comm.Parameters.AddWithValue("$id", belief.Id);
            comm.ExecuteNonQuery();
            return true;
        }

        public bool DeleteBelief(long beliefId)
        {
            SqliteCommand comm = GetNewCommand();
            comm.CommandText = @"DELETE FROM beliefs WHERE _id = $id";
            comm.Parameters.AddWithValue("$id", beliefId);
            comm.ExecuteNonQuery();
            return true;
        }

        private bool BeliefExists(long beliefId)
        {
            BeliefDTO dto = new BeliefDTO();
            SqliteCommand comm = GetNewCommand();
            comm.CommandText = @"SELECT * FROM beliefs WHERE _id = $id";
            comm.Parameters.AddWithValue("$id", beliefId);
            var reader = comm.ExecuteReader();
            return reader.HasRows;
        }

        private BeliefDTO? GetBeliefByTextTopicId(long topicId, string belief)
        {
            BeliefDTO result = new BeliefDTO();
            SqliteCommand comm = GetNewCommand();
            comm.CommandText = @"SELECT * FROM beliefs WHERE belief = @belief AND topic_id = @topicId";
            comm.Parameters.AddWithValue("@belief", belief);
            comm.Parameters.AddWithValue("@topicId", topicId);
            comm.Prepare();
            var reader = comm.ExecuteReader();
            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    result.Id = reader.GetInt64(0);
                    result.Belief = reader.GetString(1);
                    result.TopicId = reader.GetInt64(2);
                }
                return result;
            }
            else
            {
                return null;
            }
        }

        #endregion

        #region Verses
        public VerseDTO CreateVerse(VerseDTO verse)
        {
            if (!BeliefExists(verse.BeliefId))
            {
                throw new Exception("Can't add verse to non-existent belief.");
            }
            SqliteCommand comm = GetNewCommand();
            comm.CommandText = @"INSERT INTO verses(verse, belief_id, book, chapter, verseStart, verseEnd) VALUES ($verse, $belief_id, $book, $chapter, $verseStart, $verseEnd)";
            comm.Parameters.AddWithValue("$verse", verse.VerseText);
            comm.Parameters.AddWithValue("$belief_id", verse.BeliefId);
            comm.Parameters.AddWithValue("$book", verse.Book);
            comm.Parameters.AddWithValue("$chapter", verse.Chapter);
            comm.Parameters.AddWithValue("$verseStart", verse.VerseStart);
            comm.Parameters.AddWithValue("$verseEnd", verse.VerseEnd);
            comm.Prepare();
            if (comm.ExecuteNonQuery() !=1)
            {
                throw new Exception("Couldn't insert new verse.");
            }
            var result = GetVerseByText(verse.VerseText, verse.BeliefId);
            return result;
        }
                
        public BindingList<VerseDTO> GetVerses(long beliefId)
        {
            BindingList<VerseDTO> verseList = new BindingList<VerseDTO>();
            var comm = GetNewCommand();
            comm.CommandText = "SELECT * FROM Verses WHERE belief_id = $beliefId";
            comm.Parameters.AddWithValue("$beliefId", beliefId);
            var reader = comm.ExecuteReader();
            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    VerseDTO verse = new VerseDTO();
                    verse.Id = reader.GetInt64(0);
                    verse.VerseText = reader.GetString(1);
                    verse.BeliefId = reader.GetInt64(2);
                    verse.VerseEnd = reader.GetInt64(3);
                    verse.VerseStart = reader.GetInt64(4);
                    verse.VerseEnd = reader.GetInt64(5);
                    verseList.Add(verse);
                }
            }
            return verseList;
        }

        public bool UpdateVerse(VerseDTO verse)
        {
            var comm = GetNewCommand();
            comm.CommandText = @"UPDATE verses SET verse = '$verseText', belief_id = $belief_id, book = $book, chapter = $chapter, verseStart = $verseStart, verseEnd = $verseEnd WHERE _Id = $id";
            comm.Parameters.AddWithValue("$id", verse.Id);
            comm.Parameters.AddWithValue("$verseText", verse.VerseText);
            comm.Parameters.AddWithValue("$belief_id", verse.BeliefId);
            comm.Parameters.AddWithValue("$book", verse.Book);
            comm.Parameters.AddWithValue("$chapter", verse.Chapter);
            comm.Parameters.AddWithValue("$verseStart", verse.VerseStart);
            comm.Parameters.AddWithValue("$verseEnd", verse.VerseEnd);
            comm.Prepare();
            return (comm.ExecuteNonQuery() == 1);
        }

        public bool DeleteVerse(long verseId)
        {
            var comm = GetNewCommand();
            comm.CommandText = @"DELETE FROM verses WHERE _Id = $verseId";
            comm.Parameters.AddWithValue("$verseId", verseId);
            comm.Prepare();
            return (comm.ExecuteNonQuery() == 1);
        }

        private VerseDTO GetVerseByText(string verseText, long beliefID)
        {
            VerseDTO result = new VerseDTO();
            SqliteCommand comm = GetNewCommand();
            comm.CommandText = @"SELECT * FROM verses WHERE verse = $verse AND belief_id = $beliefId";
            comm.Parameters.AddWithValue("$verse", verseText);
            comm.Parameters.AddWithValue("$beliefId", beliefID);
            comm.Prepare();
            var reader = comm.ExecuteReader();
            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    result.Id = reader.GetInt64(0);
                    result.VerseText = reader.GetString(1);
                    result.BeliefId = reader.GetInt64(2);
                    result.Book = reader.GetInt64(3);
                    result.Chapter = reader.GetInt64(4);
                    result.VerseStart = reader.GetInt64(5);
                    result.VerseEnd = reader.GetInt64(6);
                }
            }
            else
            {
                throw new Exception($"Unable to find verse: {verseText}");
            }
            return result;
        }
        #endregion
    }
}
