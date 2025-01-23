-- Creating MST_User table
------------------------------select----------------------------------------
select * from MST_User
select * from MST_Quiz
select * from MST_Question
select * from MST_QuestionLevel
select * from MST_QuizWiseQuestions






CREATE TABLE MST_User (
    UserID INT PRIMARY KEY IDENTITY(1,1),
    UserName NVARCHAR(100) NOT NULL,
    Password NVARCHAR(100) NOT NULL,
    Email NVARCHAR(100) NOT NULL,
    Mobile NVARCHAR(100) NOT NULL,
    IsActive BIT NOT NULL DEFAULT 1,
    IsAdmin BIT NOT NULL DEFAULT 0,
    Created DATETIME DEFAULT GETDATE(),
    Modified DATETIME DEFAULT GETDATE()
);
-- Creating MST_Quiz table
CREATE TABLE MST_Quiz (
    QuizID INT PRIMARY KEY IDENTITY(1,1),
    QuizName NVARCHAR(100) NOT NULL,
    TotalQuestions INT NOT NULL,
    QuizDate DATETIME NOT NULL,
    UserID INT NOT NULL,
    Created DATETIME NOT NULL DEFAULT GETDATE(),
    Modified DATETIME  DEFAULT GETDATE(),
    FOREIGN KEY (UserID) REFERENCES MST_User(UserID) -- Referencing MST_User table
);

-- Creating MST_Question table
CREATE TABLE MST_Question (
    QuestionID INT PRIMARY KEY IDENTITY(1,1),
    QuestionText NVARCHAR(MAX) NOT NULL,
    QuestionLevelID INT NOT NULL,
    OptionA NVARCHAR(100) NOT NULL,
    OptionB NVARCHAR(100) NOT NULL,
    OptionC NVARCHAR(100) NOT NULL,
    OptionD NVARCHAR(100) NOT NULL,
    CorrectOption NVARCHAR(100) NOT NULL,
    QuestionMarks INT NOT NULL,
    IsActive BIT NOT NULL DEFAULT 1,
    UserID INT NOT NULL,
    Created DATETIME DEFAULT GETDATE(),
    Modified DATETIME DEFAULT GETDATE(),
    FOREIGN KEY (UserID) REFERENCES MST_User(UserID),
    FOREIGN KEY (QuestionLevelID) REFERENCES MST_QuestionLevel(QuestionLevelID) -- Assuming MST_QuestionLevel exists
);

-- Creating MST_QuestionLevel table
CREATE TABLE MST_QuestionLevel (
    QuestionLevelID INT PRIMARY KEY IDENTITY(1,1),
    QuestionLevel NVARCHAR(100) NOT NULL,
    UserID INT NOT NULL,
    Created DATETIME NOT NULL DEFAULT GETDATE(),
    Modified DATETIME  DEFAULT GETDATE(),
    FOREIGN KEY (UserID) REFERENCES MST_User(UserID) -- Referencing MST_User table
);

-- Creating MST_QuizWiseQuestions table
CREATE TABLE MST_QuizWiseQuestions (
    QuizWiseQuestionsID INT PRIMARY KEY IDENTITY(1,1),
    QuizID INT NOT NULL,
    QuestionID INT NOT NULL,
    UserID INT NOT NULL,
    Created DATETIME NOT NULL DEFAULT GETDATE(),
    Modified DATETIME DEFAULT GETDATE(),
    FOREIGN KEY (QuizID) REFERENCES MST_Quiz(QuizID),        -- Referencing MST_Quiz table
    FOREIGN KEY (QuestionID) REFERENCES MST_Question(QuestionID),  -- Referencing MST_Question table
    FOREIGN KEY (UserID) REFERENCES MST_User(UserID)         -- Referencing MST_User table
);
select * from MST_QuizWiseQuestions


----------------------------------------------store procedure------------------------------------------------------------

-- STORED PROCEDURES FOR USERS TABLE --

-- Select All Users
-- Stored Procedure: Select All Users
CREATE PROCEDURE Sp_MST_User_SelectAllUsers
AS
BEGIN
    SELECT UserID, UserName, Email, Mobile, IsActive, IsAdmin, Created, Modified
    FROM MST_User;
END;

-- Select User by ID
CREATE PROCEDURE Sp_MST_User_GetUser_ByID

    @UserID INT
AS
BEGIN
    SELECT UserID, UserName, Email, Mobile, IsActive, IsAdmin, Created, Modified
    FROM MST_User
    WHERE UserID = @UserID;
END;


 -- Stored Procedure: Insert a New User
CREATE PROCEDURE  Sp_MST_User_InsertNewUser
    @UserName NVARCHAR(100),
    @Password NVARCHAR(100),
    @Email NVARCHAR(100),
    @Mobile NVARCHAR(100),
    @IsActive BIT = 1,
    @IsAdmin BIT = 0
AS
BEGIN
    INSERT INTO MST_User (UserName, Password, Email, Mobile, IsActive, IsAdmin, Created, Modified)
    VALUES (@UserName, @Password, @Email, @Mobile, @IsActive, @IsAdmin, GETDATE(), GETDATE());
END;

-- Stored Procedure: Update an Existing User
CREATE PROCEDURE Sp_MST_User_UpdateUser
    @UserID INT,
    @UserName NVARCHAR(100),
    @Password NVARCHAR(100),
    @Email NVARCHAR(100),
    @Mobile NVARCHAR(100),
    @IsActive BIT,
    @IsAdmin BIT
AS
BEGIN
    UPDATE MST_User
    SET UserName = @UserName,
        Password = @Password,
        Email = @Email,
        Mobile = @Mobile,
        IsActive = @IsActive,
        IsAdmin = @IsAdmin,
        Modified = GETDATE()
    WHERE UserID = @UserID;
END;


-- Stored Procedure: Delete a User by ID
CREATE PROCEDURE Sp_MST_User_DeleteUserByID
    @UserID INT
AS
BEGIN
    DELETE FROM MST_User
    WHERE UserID = @UserID;
END;

-----------------Exicute Store procedure----------------------------
--1
EXEC Sp_MST_User_SelectAllUsers;
--2
EXEC Sp_MST_User_GetUser_ByID @UserID = 1;
--3
EXEC Sp_MST_User_InsertNewUser 
    @UserName = 'afsana', 
    @Password = 'Password456', 
    @Email = 'ghada@example.com', 
    @Mobile = '987-654-3210', 
    @IsActive = 1, 
    @IsAdmin = 0;
EXEC Sp_MST_User_InsertNewUser 
    @UserName = 'Ruchi', 
    @Password = 'Pass456', 
    @Email = 'parmar@example.com', 
    @Mobile = '111-654-3210', 
    @IsActive = 1, 
    @IsAdmin = 0;
--4
EXEC Sp_MST_User_UpdateUser 
    @UserID = 1, 
    @UserName = 'asma', 
    @Password = 'NewPassword123', 
    @Email = 'smith@example.com', 
    @Mobile = '555-555-5555', 
    @IsActive = 1, 
    @IsAdmin = 0;
--5
EXEC Sp_MST_User_DeleteUserByID @UserID = 1;

-------------------------------Store Procedure for MST_Quiz---------------------------

-- Stored Procedure: Select All Quizzes
CREATE PROCEDURE Sp_MST_Quiz_SelectAllQuizzes
AS
BEGIN
    SELECT QuizID, QuizName, TotalQuestions, QuizDate, UserID, Created, Modified
    FROM MST_Quiz;
END;

-- Stored Procedure: Select Quiz by ID
CREATE PROCEDURE Sp_MST_Quiz_SelectQuizByID
    @QuizID INT
AS
BEGIN
    SELECT QuizID, QuizName, TotalQuestions, QuizDate, UserID, Created, Modified
    FROM MST_Quiz
    WHERE QuizID = @QuizID;
END;

-- Stored Procedure: Insert a New Quiz
CREATE PROCEDURE Sp_MST_Quiz_InsertNewQuiz
    @QuizName NVARCHAR(100),
    @TotalQuestions INT,
    @QuizDate DATETIME,
    @UserID INT
AS
BEGIN
    INSERT INTO MST_Quiz (QuizName, TotalQuestions, QuizDate, UserID, Created, Modified)
    VALUES (@QuizName, @TotalQuestions, @QuizDate, @UserID, GETDATE(), GETDATE());
END;

-- Stored Procedure: Update an Existing Quiz
CREATE PROCEDURE Sp_MST_Quiz_UpdateQuiz
    @QuizID INT,
    @QuizName NVARCHAR(100),
    @TotalQuestions INT,
    @QuizDate DATETIME,
    @UserID INT
AS
BEGIN
    UPDATE MST_Quiz
    SET QuizName = @QuizName,
        TotalQuestions = @TotalQuestions,
        QuizDate = @QuizDate,
        UserID = @UserID,
        Modified = GETDATE()
    WHERE QuizID = @QuizID;
END;

-- Stored Procedure: Delete a Quiz by ID
CREATE PROCEDURE Sp_MST_Quiz_DeleteQuizByID
    @QuizID INT
AS
BEGIN
    DELETE FROM MST_Quiz
    WHERE QuizID = @QuizID;
END;

--------------Exicute-------
-- 1. Execute SelectAllQuizzes (Select All Quizzes)
EXEC Sp_MST_Quiz_SelectAllQuizzes;

-- 2. Execute SelectQuizByID (Select Quiz by ID)
EXEC Sp_MST_Quiz_SelectQuizByID @QuizID = 1;  

-- 3. Execute InsertNewQuiz (Insert a New Quiz)
EXEC Sp_MST_Quiz_InsertNewQuiz 
    @QuizName = 'Math Quiz', 
    @TotalQuestions = 10, 
    @QuizDate = '2025-02-01 10:00:00', 
    @UserID = 1;
EXEC Sp_MST_Quiz_InsertNewQuiz 
    @QuizName = 'CP Quiz', 
    @TotalQuestions = 10, 
    @QuizDate = '2025-11-01 12:00:00', 
    @UserID = 1;
-- 4. Execute UpdateQuiz (Update an Existing Quiz)
EXEC Sp_MST_Quiz_UpdateQuiz 
    @QuizID = 1, 
    @QuizName = 'Updated Math Quiz', 
    @TotalQuestions = 15, 
    @QuizDate = '2025-02-05 10:00:00', 
    @UserID = 1;

--5. Execute DeleteQuizByID (Delete a Quiz by ID)
EXEC Sp_MST_Quiz_DeleteQuizByID @QuizID = 1;  

------------------------------- Store procedure for MST_Question------------------------------

-- Stored Procedure: Select All Questions
CREATE PROCEDURE Sp_MST_Question_SelectAllQuestions
AS
BEGIN
    SELECT QuestionID, QuestionText, QuestionLevelID, OptionA, OptionB, OptionC, OptionD, CorrectOption, QuestionMarks, IsActive, UserID, Created, Modified
    FROM MST_Question;
END;

-- Stored Procedure: Select Question by ID
CREATE PROCEDURE Sp_MST_Question_SelectQuestionByID
    @QuestionID INT
AS
BEGIN
    SELECT QuestionID, QuestionText, QuestionLevelID, OptionA, OptionB, OptionC, OptionD, CorrectOption, QuestionMarks, IsActive, UserID, Created, Modified
    FROM MST_Question
    WHERE QuestionID = @QuestionID;
END;

-- Stored Procedure: Insert a New Question
CREATE PROCEDURE Sp_MST_Question_InsertNewQuestion
    @QuestionText NVARCHAR(MAX),
    @QuestionLevelID INT,
    @OptionA NVARCHAR(100),
    @OptionB NVARCHAR(100),
    @OptionC NVARCHAR(100),
    @OptionD NVARCHAR(100),
    @CorrectOption NVARCHAR(100),
    @QuestionMarks INT,
    @UserID INT,
    @IsActive BIT = 1
AS
BEGIN
    INSERT INTO MST_Question (QuestionText, QuestionLevelID, OptionA, OptionB, OptionC, OptionD, CorrectOption, QuestionMarks, IsActive, UserID, Created, Modified)
    VALUES (@QuestionText, @QuestionLevelID, @OptionA, @OptionB, @OptionC, @OptionD, @CorrectOption, @QuestionMarks, @IsActive, @UserID, GETDATE(), GETDATE());
END;

-- Stored Procedure: Update an Existing Question
CREATE PROCEDURE Sp_MST_Question_UpdateQuestion
    @QuestionID INT,
    @QuestionText NVARCHAR(MAX),
    @QuestionLevelID INT,
    @OptionA NVARCHAR(100),
    @OptionB NVARCHAR(100),
    @OptionC NVARCHAR(100),
    @OptionD NVARCHAR(100),
    @CorrectOption NVARCHAR(100),
    @QuestionMarks INT,
    @IsActive BIT,
    @UserID INT
AS
BEGIN
    UPDATE MST_Question
    SET QuestionText = @QuestionText,
        QuestionLevelID = @QuestionLevelID,
        OptionA = @OptionA,
        OptionB = @OptionB,
        OptionC = @OptionC,
        OptionD = @OptionD,
        CorrectOption = @CorrectOption,
        QuestionMarks = @QuestionMarks,
        IsActive = @IsActive,
        UserID = @UserID,
        Modified = GETDATE()
    WHERE QuestionID = @QuestionID;
END;
-- Stored Procedure: Delete a Question by ID
CREATE PROCEDURE Sp_MST_Question_DeleteQuestionByID
    @QuestionID INT
AS
BEGIN
    DELETE FROM MST_Question
    WHERE QuestionID = @QuestionID;
END;

--------------------------------Exicute------------------------------

-- 1. Execute SelectAllQuestions (Select All Questions)
EXEC Sp_MST_Question_SelectAllQuestions;

-- 2. Execute SelectQuestionByID (Select Question by ID)
EXEC Sp_MST_Question_SelectQuestionByID @QuestionID = 1;  -- Replace '1' with the desired QuestionID

-- 3. Execute InsertNewQuestion (Insert a New Question)
EXEC Sp_MST_Question_InsertNewQuestion 
    @QuestionText = 'What is 2 + 2?', 
    @QuestionLevelID = 3,  
    @OptionA = '3', 
    @OptionB = '4', 
    @OptionC = '5', 
    @OptionD = '6', 
    @CorrectOption = 'B', 
    @QuestionMarks = 1, 
    @UserID = 1;  -- Replace with the desired UserID

	-- 3. Execute InsertNewQuestion (Insert a New Question)
EXEC Sp_MST_Question_InsertNewQuestion 
    @QuestionText = 'What is Capital Of India?', 
    @QuestionLevelID = 5,  
    @OptionA = 'Gujrat', 
    @OptionB = 'Bihar', 
    @OptionC = 'New Delhi', 
    @OptionD = 'Jammu', 
    @CorrectOption = 'c', 
    @QuestionMarks = 1, 
    @UserID = 2;  -- Replace with the desired UserID

-- 4. Execute UpdateQuestion (Update an Existing Question)
EXEC Sp_MST_Question_UpdateQuestion 
    @QuestionID = 1, 
    @QuestionText = 'What is 3 + 3?', 
    @QuestionLevelID = 1, 
    @OptionA = '5', 
    @OptionB = '6', 
    @OptionC = '7', 
    @OptionD = '8', 
    @CorrectOption = 'B', 
    @QuestionMarks = 1, 
    @IsActive = 1, 
    @UserID = 1;

-- 5. Execute DeleteQuestionByID (Delete a Question by ID)
EXEC Sp_MST_Question_DeleteQuestionByID @QuestionID = 1;  -- Replace '1' with the desired QuestionID

----------------------------------------Store procudere for MST_QuestionLevel---------------------------------

-- Stored Procedure: Select All Question Levels
Create PROCEDURE Sp_MST_QuestionLevel_SelectAll
AS
BEGIN
    SELECT QuestionLevelID, QuestionLevel, UserID, Created, Modified
    FROM MST_QuestionLevel;
END;


-- Stored Procedure: Select Question Level by ID
CREATE PROCEDURE Sp_MST_QuestionLevel_SelectByID
    @QuestionLevelID INT
AS
BEGIN
    SELECT QuestionLevelID, QuestionLevel, UserID, Created, Modified
    FROM MST_QuestionLevel
    WHERE QuestionLevelID = @QuestionLevelID;
END;

-- Stored Procedure: Insert a New Question Level
CREATE PROCEDURE Sp_MST_QuestionLevel_Insert
    @QuestionLevel NVARCHAR(100),
    @UserID INT
AS
BEGIN
    INSERT INTO MST_QuestionLevel (QuestionLevel, UserID, Created, Modified)
    VALUES (@QuestionLevel, @UserID, GETDATE(), GETDATE());
END


DROP PROCEDURE IF EXISTS Sp_MST_QuestionLevel_Insert;


-- Stored Procedure: Update an Existing Question Level
CREATE PROCEDURE Sp_MST_QuestionLevel_Update
    @QuestionLevelID INT,
    @QuestionLevel NVARCHAR(100),
    @UserID INT
AS
BEGIN
    UPDATE MST_QuestionLevel
    SET QuestionLevel = @QuestionLevel,
        UserID = @UserID,
        Modified = GETDATE()
    WHERE QuestionLevelID = @QuestionLevelID;
END;

-- Stored Procedure: Delete a Question Level by ID
CREATE PROCEDURE Sp_MST_QuestionLevel_DeleteByID
    @QuestionLevelID INT
AS
BEGIN
    DELETE FROM MST_QuestionLevel
    WHERE QuestionLevelID = @QuestionLevelID;
END;

-----------------------exicute --------------------------
EXEC Sp_MST_QuestionLevel_SelectAll;
EXEC Sp_MST_QuestionLevel_SelectByID @QuestionLevelID = 1;
EXEC Sp_MST_QuestionLevel_Insert @QuestionLevel = 'Basic', @UserID = 2;

EXEC Sp_MST_QuestionLevel_Update @QuestionLevelID = 4, @QuestionLevel = 'Expert', @UserID = 1;
EXEC Sp_MST_QuestionLevel_DeleteByID @QuestionLevelID = 4;


----------------------------------------- stpre procedure for QuizWiseQuestions------------------------------------



-- Stored Procedure: Select All QuizWiseQuestions
CREATE PROCEDURE Sp_MST_QuizWiseQuestions_SelectAll
AS
BEGIN
    SELECT QuizWiseQuestionsID, QuizID, QuestionID, UserID, Created, Modified
    FROM MST_QuizWiseQuestions;
END;

-- Stored Procedure: Select QuizWiseQuestions by ID
CREATE PROCEDURE Sp_MST_QuizWiseQuestions_SelectByID
    @QuizWiseQuestionsID INT
AS
BEGIN
    SELECT QuizWiseQuestionsID, QuizID, QuestionID, UserID, Created, Modified
    FROM MST_QuizWiseQuestions
    WHERE QuizWiseQuestionsID = @QuizWiseQuestionsID;
END;

-- Stored Procedure: Insert a New QuizWiseQuestion
CREATE PROCEDURE Sp_MST_QuizWiseQuestions_Insert
    @QuizID INT,
    @QuestionID INT,
    @UserID INT
AS
BEGIN
    INSERT INTO MST_QuizWiseQuestions (QuizID, QuestionID, UserID, Created, Modified)
    VALUES (@QuizID, @QuestionID, @UserID, GETDATE(), GETDATE());
END;

-- Stored Procedure: Update an Existing QuizWiseQuestion
CREATE PROCEDURE Sp_MST_QuizWiseQuestions_Update
    @QuizWiseQuestionsID INT,
    @QuizID INT,
    @QuestionID INT,
    @UserID INT
AS
BEGIN
    UPDATE MST_QuizWiseQuestions
    SET QuizID = @QuizID,
        QuestionID = @QuestionID,
        UserID = @UserID,
        Modified = GETDATE()
    WHERE QuizWiseQuestionsID = @QuizWiseQuestionsID;
END;

-- Stored Procedure: Delete a QuizWiseQuestion by ID
CREATE PROCEDURE Sp_MST_QuizWiseQuestions_DeleteByID
    @QuizWiseQuestionsID INT
AS
BEGIN
    DELETE FROM MST_QuizWiseQuestions
    WHERE QuizWiseQuestionsID = @QuizWiseQuestionsID;
END;


--------------------------------Exicute-------------------------------

EXEC Sp_MST_QuizWiseQuestions_SelectAll;
EXEC Sp_MST_QuizWiseQuestions_SelectByID @QuizWiseQuestionsID = 1;
EXEC Sp_MST_QuizWiseQuestions_Insert @QuizID = 1, @QuestionID = 4, @UserID = 2;
EXEC Sp_MST_QuizWiseQuestions_Update @QuizWiseQuestionsID = 2, @QuizID = 2, @QuestionID = 3, @UserID = 1;
EXEC Sp_MST_QuizWiseQuestions_DeleteByID @QuizWiseQuestionsID = 2;
