using System.Collections;
using System.Runtime.InteropServices;
using UnityEngine;
using System.Collections.Generic;

namespace Quiz
{
    public class SystemManager : MonoBehaviour
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        [DllImport("__Internal")]
        private static extern void NotifyReactApp(string message);
#endif

        public void SetIsStudent(string isStudent)
        {
            Debug.Log("SetIsStudent: " + isStudent);
            Managers.Quiz.isStudent = isStudent == "true";
        }

        public void SetQuizData(string quizData)
        {
            Debug.Log(quizData);
            QuizData response = JsonUtility.FromJson<QuizData>(quizData);
            for (int i = 0; i < response.courseContent.courseContentDetails.Count; i++)
            {
                if (response.courseContent.courseContentDetails[i].activityStage == "EXPERIMENT")
                {
                    Managers.Quiz.SetQuizData(response.courseContent.courseContentDetails[i]);
                }
            }
        }

        public void SetQuizReport(string quizReport)
        {
            Debug.Log(quizReport);
            StudentQuizData response = JsonUtility.FromJson<StudentQuizData>(quizReport);
            Managers.Quiz.SetSolveQuizData(response.quizzesByStageAndStep);
        }

        public void SetShortQuizFeedback(string feedback)
        {
            Managers.Quiz.SetFeedback(feedback);
        }

        public void GetNextQuiz()
        {
            Managers.Quiz.OpenNextQuiz();
        }

        public void GetPreviousQuiz()
        {
            Managers.Quiz.OpenPreviousQuiz();
        }

        public void CallNotifyReactApp(string message)
        {
#if UNITY_WEBGL && !UNITY_EDITOR
            NotifyReactApp(message);
#endif
        }

        // 더미 데이터 생성 메서드
        public void SetDummyQuizData()
        {
            Managers.Quiz.isStudent = true;
            // 퀴즈 데이터 설정
            CourseContentDetail quizData = new CourseContentDetail
            {
                id = 219,
                step = 3,
                activityStage = "EXPERIMENT",
                multipleChoiceQuizzes = new List<MultipleChoiceQuizData>
                {
                    new MultipleChoiceQuizData
                    {
                        id = 1,
                        uuid = "8d776784-01c5-47ff-b444-41370f1e77ae",
                        quizStep = 3,
                        question = "화산활동에 필요한 재료중 아닌것은",
                        firstOption = "화산암",
                        firstFeedback = "정답",
                        secondOption = "꼬소",
                        secondFeedback = "오답",
                        thirdOption = "물티슈",
                        thirdFeedback = "오답",
                        fourthOption = "차키",
                        fourthFeedback = "오답",
                        correctOption = 1
                    }
                },
                oxChoiceQuizzes = new List<OxChoiceQuizData>
                {
                    new OxChoiceQuizData
                    {
                        id = 1,
                        uuid = "49da9972-88a9-483c-be16-40a4759ef1ea",
                        quizStep = 2,
                        question = "화산은 위험할까요",
                        firstOption = "네",
                        firstFeedback = "정답입니다",
                        secondOption = "아니오",
                        secondFeedback = "오답",
                        correctOption = 1
                    }
                },
                shortQuizzes = new List<ShortQuizData>
                {
                    new ShortQuizData
                    {
                        id = 1,
                        uuid = "2397e919-f102-4301-9671-be878beb265e",
                        quizStep = 5,
                        question = "화산활동 모형에서 {{}}은 마그마를 표현한 것이다",
                        rubric = "화산활동 모형에대한 질문 정답은 마시멜로"
                    }
                },
                classificationQuizzes = new List<ClassificationQuizData>
                {
                    new ClassificationQuizData
                    {
                        id = 1,
                        uuid = "49d6d71e-2156-4f89-bfa8-c897c642c0b6",
                        quizStep = 4,
                        question = "다리의 유무",
                        description = "분류 기준으로 동물 카드 분류를 해봅시다",
                        correctFeedback = "분류에대한 정답 피드백 ",
                        incorrectFeedback = "분류에 대한 오답 피드백",
                        sections = new List<Section>
                        {
                            new Section
                            {
                                id = 2,
                                question = "포유류",
                                correctItemIds = new List<int> { 1, 2, 5 }
                            },
                            new Section
                            {
                                id = 1,
                                question = "파충류",
                                correctItemIds = new List<int> { 3, 4, 6 }
                            }
                        },
                        sectionItems = new List<SectionItem>
                        {
                            new SectionItem
                            {
                                id = 1,
                                name = "꼬소",
                                imageUrl = "https://picsum.photos/200"
                            },
                            new SectionItem
                            {
                                id = 2,
                                name = "호랑이",
                                imageUrl = "https://picsum.photos/id/237/200/300"
                            },
                            new SectionItem
                            {
                                id = 3,
                                name = "사마귀",
                                imageUrl = "https://picsum.photos/seed/picsum/200/300"
                            },
                            new SectionItem
                            {
                                id = 4,
                                name = "바선생",
                                imageUrl = "https://picsum.photos/200/300?grayscale"
                            },
                            new SectionItem
                            {
                                id = 5,
                                name = "말",
                                imageUrl = "https://picsum.photos/200/300/?blur"
                            },
                            new SectionItem
                            {
                                id = 6,
                                name = "개미",
                                imageUrl = "https://picsum.photos/id/870/200/300?grayscale&blur=2"
                            }
                        }
                    }
                },
                blankQuizzes = new List<BlankQuizData>
                {
                    new BlankQuizData
                    {
                        id = 1,
                        uuid = "68da67a2-aefb-44ac-b8b6-2b81027dffe3",
                        quizStep = 1,
                        question = "화산활동 모형에서 {{}}은 마그마를 표현한 것이다",
                        firstOption = "마시멜로",
                        firstFeedback = "정답입니다",
                        secondOption = "꼬소",
                        secondFeedback = "오답입니다",
                        thirdOption = "화산암",
                        thirdFeedback = "오답입니다",
                        correctOption = 1
                    }
                }
            };

            // 해결된 퀴즈 데이터 설정
            List<QuizByStageAndStep> solvedQuizData = new List<QuizByStageAndStep>
            {
                new QuizByStageAndStep
                {
                    activityStage = "EXPERIMENT",
                    step = 2,
                    oxQuiz = new OxQuiz
                    {
                        studentId = 354,
                        step = 2,
                        question = "화산은 위험할까요",
                        firstOption = "네",
                        secondOption = "아니오",
                        firstFeedback = "정답입니다",
                        secondFeedback = "오답",
                        activityStage = "EXPERIMENT",
                        id = 3,
                        studentAnswer = "1",
                        isCorrect = true,
                        correctAnswer = "1",
                        submittedAt = "2025-05-07T07:57:37.542Z",
                        timeTaken = 10
                    }
                },
                new QuizByStageAndStep
                {
                    activityStage = "EXPERIMENT",
                    step = 2,
                    multipleChoiceQuiz = new MultipleChoiceQuiz
                    {
                        studentId = 354,
                        step = 3,
                        question = "화산활동에 필요한 재료중 아닌것은",
                        firstOption = "화산암",
                        secondOption = "꼬소",
                        thirdOption = "물티슈",
                        fourthOption = "차키",
                        firstFeedback = "정답",
                        secondFeedback = "오답",
                        thirdFeedback = "오답",
                        fourthFeedback = "오답",
                        activityStage = "EXPERIMENT",
                        id = 42,
                        studentAnswer = "1",
                        isCorrect = true,
                        submittedAt = "2025-05-07T11:52:35.213Z",
                        timeTaken = 10
                    }
                },
                new QuizByStageAndStep
                {
                    activityStage = "EXPERIMENT",
                    step = 2,
                    blankQuiz = new BlankQuiz
                    {
                        studentId = 356,
                        step = 1,
                        question = "화산활동에서 {{}}은 마그마를 표현한 것이다",
                        firstOption = "마시멜로",
                        secondOption = "꼬소",
                        thirdOption = "화산암",
                        firstFeedback = "정답입니다",
                        secondFeedback = "오답입니다",
                        thirdFeedback = "틀렸습니다",
                        activityStage = "EXPERIMENT",
                        id = 2,
                        studentAnswer = "1",
                        isCorrect = false,
                        correctAnswer = 2,
                        submittedAt = "2025-05-07T11:52:54.727Z",
                        timeTaken = 10
                    }
                },
                new QuizByStageAndStep
                {
                    activityStage = "EXPERIMENT",
                    step = 2,
                    shortQuiz = new ShortQuiz
                    {
                        studentId = 356,
                        step = 5,
                        question = "화산활동 모형에서 {{}}은 마그마를 표현한 것이다",
                        correctAnswer = "hello",
                        activityStage = "EXPERIMENT",
                        feedbackSample = "",
                        id = 37,
                        studentAnswer = "마시멜로로",
                        isCorrect = true,
                        feedback = "정답입니다! 마시멜로는 화산활동 모형에서 마그마를 표현하는 데 사용됩니다. 잘했어요!",
                        submittedAt = "2025-05-07T12:58:20.260Z",
                        timeTaken = 22
                    }
                }
            };

            // 데이터 설정
            Managers.Quiz.SetQuizData(quizData);
            Managers.Quiz.SetSolveQuizData(solvedQuizData);
        }
    }
}
