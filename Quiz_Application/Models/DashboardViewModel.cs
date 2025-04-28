using System.Collections.Generic;

namespace QuizApplication.Models
{
    public class DashboardViewModel
    {
        public int TotalQuizzes { get; set; }
        public int TotalQuestions { get; set; }
        public int TotalQuestionLevels { get; set; }
        public List<QuizModel> RecentQuizzes { get; set; } = new List<QuizModel>();
        public List<QuestionModel> RecentQuestions { get; set; } = new List<QuestionModel>();
    }

    public class QuizModel
    {
        public int QuizID { get; set; }
        public string QuizName { get; set; }
        public int TotalQuestions { get; set; }
        public string QuizDate { get; set; }
    }

    public class QuestionModel
    {
        public int QuestionID { get; set; }
        public string QuestionText { get; set; }
        public int QuestionMarks { get; set; }
        public string CorrectOption { get; set; }
    }
}
