using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Quiz
{
    public class ClassificationQuizType : QuizBase, IQuiz
    {
        [SerializeField] private TMP_Text firstSectionText;
        [SerializeField] private TMP_Text secondSectionText;
        [SerializeField] private GameObject classificationFlagObject;
        [SerializeField] private TMP_Text descriptionText;
        [SerializeField] private List<Sprite> sectionCorrectSprite;
        [SerializeField] private GameObject[] sectionBox;
        [SerializeField] private GameObject sectionItemBox;
        [SerializeField] private GameObject root;
        private ClassificationQuizData quizData;
        private List<Section> sections;
        private List<SectionItem> sectionItems;

        private bool isCorrect = false;

        public void BuildQuiz()
        {
            InitQuizUI();
            submitButton.onClick.RemoveAllListeners();
            submitButton.onClick.AddListener(SubmitQuiz);
            isCheckTime = true;
            root.GetComponent<RectTransform>().sizeDelta = new Vector2(1500, 869.92f);
            submitObject.GetComponent<RectTransform>().sizeDelta = new Vector2(1420, 100);
            nextObject.GetComponent<RectTransform>().sizeDelta = new Vector2(1420, 100);
            feedback.gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(1420, 148.2f);
            sectionItemBox.SetActive(true);
            submitButton.interactable = true;
        }

        public void SetQuizData(object quizData)
        {
            this.quizData = (ClassificationQuizData)quizData;
            id = this.quizData.id;
            uuid = this.quizData.uuid;
            quizStep = this.quizData.quizStep;
            question = this.quizData.question;
            sections = this.quizData.sections;
            sectionItems = this.quizData.sectionItems;
            classificationFlagObject.GetComponentInChildren<TMP_Text>().text = this.quizData.question;
            classificationFlagObject.SetActive(true);
            descriptionText.text = this.quizData.description;
            firstSectionText.text = sections[0].question;
            secondSectionText.text = sections[1].question;

            for (int i = 0; i < sectionItems.Count; i++)
            {
                GameObject sectionItem = Managers.Resource.Instantiate("SectionItem", sectionItemBox.transform);
                sectionItem.GetComponent<DragSectionItem>().SetSectionItem(sectionItems[i]);
                sectionItem.GetComponent<DragSectionItem>().rootParent = transform;
            }
        }

        public void SubmitQuiz()
        {
            isCorrect = true;

            // 각 Section에 대해 정답 확인
            for (int i = 0; i < sections.Count; i++)
            {
                Section currentSection = sections[i];
                GameObject sectionBox = this.sectionBox[i];

                // 현재 Section에 있는 아이템들의 ID 수집
                List<int> currentItemIds = new List<int>();
                foreach (Transform child in sectionBox.transform)
                {
                    DragSectionItem sectionItem = child.GetComponent<DragSectionItem>();
                    if (sectionItem != null)
                    {
                        currentItemIds.Add(sectionItem.id);
                    }
                }

                // 정답 ID 리스트와 현재 아이템 ID 리스트 비교
                isCorrect = CompareLists(currentSection.correctItemIds, currentItemIds);
                Debug.Log($"Section {i + 1} ({currentSection.question}) 정답 여부: {isCorrect}");
                ResponseFeedback("");
                nextObject.SetActive(true);
            }
        }

        private bool CompareLists(List<int> list1, List<int> list2)
        {
            if (list1.Count != list2.Count)
                return false;

            // 두 리스트를 정렬하여 비교
            List<int> sortedList1 = new List<int>(list1);
            List<int> sortedList2 = new List<int>(list2);

            sortedList1.Sort();
            sortedList2.Sort();

            for (int i = 0; i < sortedList1.Count; i++)
            {
                if (sortedList1[i] != sortedList2[i])
                    return false;
            }

            return true;
        }

        public void SetSolvedQuizData(object quizData)
        {
            if (quizData is ClassificationQuiz data)
            {
                // 모든 SectionItem을 원래 위치로 초기화
                foreach (Transform child in sectionItemBox.transform)
                {
                    DragSectionItem sectionItem = child.GetComponent<DragSectionItem>();
                    if (sectionItem != null)
                    {
                        sectionItem.transform.SetParent(sectionItemBox.transform);
                        sectionItem.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
                    }
                }

                // StudentAnswer에 따라 아이템들을 각 Section으로 이동
                foreach (var sectionAnswer in data.studentAnswer)
                {
                    // sectionAnswer의 키값과 일치하는 SectionBox 찾기
                    GameObject targetSectionBox = null;
                    for (int i = 0; i < sectionBox.Length; i++)
                    {
                        TMP_Text sectionText = i == 0 ? firstSectionText : secondSectionText;
                        if (sectionText.text == sectionAnswer.Key)
                        {
                            targetSectionBox = sectionBox[i];
                            break;
                        }
                    }

                    // 일치하는 SectionBox가 있는 경우에만 아이템 이동
                    if (targetSectionBox != null)
                    {
                        // 학생이 선택한 아이템들을 해당 Section으로 이동
                        foreach (var item in sectionAnswer.Value)
                        {
                            foreach (Transform child in sectionItemBox.transform)
                            {
                                DragSectionItem sectionItem = child.GetComponent<DragSectionItem>();
                                if (sectionItem != null && sectionItem.itemName == item.name)
                                {
                                    sectionItem.transform.SetParent(targetSectionBox.transform);
                                    sectionItem.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
                                    break;
                                }
                            }
                        }
                    }
                    else
                    {
                        Debug.LogWarning($"Section '{sectionAnswer.Key}'에 해당하는 SectionBox를 찾을 수 없습니다.");
                    }
                }

                // 정답 여부 확인
                isCorrect = data.isCorrect;
                ResponseFeedback(isCorrect ? data.correctFeedback : data.inCorrectFeedback);
                nextObject.SetActive(true);
                submitObject.SetActive(false);
                sectionItemBox.SetActive(false);
            }
        }

        public void ResponseFeedback(string responseData)
        {
            if (isCorrect)
            {
                feedback.SetFeedback(quizData.correctFeedback, isCorrect);
            }
            else
            {
                feedback.SetFeedback(quizData.incorrectFeedback, isCorrect);
            }
            feedback.gameObject.SetActive(true);
            submitObject.SetActive(false);
            sectionItemBox.SetActive(false);

            if (isCorrect)
            {
                sectionBox[0].GetComponent<Image>().sprite = sectionCorrectSprite[0];
                sectionBox[1].GetComponent<Image>().sprite = sectionCorrectSprite[1];
            }
            else
            {
                sectionBox[0].GetComponent<Image>().sprite = sectionCorrectSprite[2];
                sectionBox[1].GetComponent<Image>().sprite = sectionCorrectSprite[2];
            }
        }

        public void ShowTeacherView()
        {
            // 모든 SectionItem을 원래 위치로 초기화
            foreach (Transform child in sectionItemBox.transform)
            {
                DragSectionItem sectionItem = child.GetComponent<DragSectionItem>();
                if (sectionItem != null)
                {
                    sectionItem.transform.SetParent(sectionItemBox.transform);
                    sectionItem.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
                }
            }

            // 각 Section에 맞는 아이템들을 이동
            for (int i = 0; i < sections.Count; i++)
            {
                Section currentSection = sections[i];
                GameObject targetSectionBox = sectionBox[i];

                // 현재 Section의 정답 아이템들을 찾아서 이동
                foreach (int correctItemId in currentSection.correctItemIds)
                {
                    // sectionItemBox에서 해당 ID를 가진 아이템 찾기
                    foreach (Transform child in sectionItemBox.transform)
                    {
                        DragSectionItem sectionItem = child.GetComponent<DragSectionItem>();
                        if (sectionItem != null && sectionItem.id == correctItemId)
                        {
                            // 아이템을 해당 Section으로 이동
                            sectionItem.transform.SetParent(targetSectionBox.transform);
                            sectionItem.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
                            break;
                        }
                    }
                }
            }

            // 정답 상태로 UI 업데이트
            isCorrect = true;
            ResponseFeedback("");
            nextObject.SetActive(false);
            submitObject.SetActive(false);
            sectionItemBox.SetActive(false);
        }

        public void TestClassificationQuizDummyData()
        {
            ClassificationQuizData quizData = new ClassificationQuizData
            {
                id = 1,
                uuid = "49d6d71e-2156-4f89-bfa8-c897c642c0b6",
                quizStep = 4,
                question = "다리의 유무",
                description = "분류 기준으로 동물 카드 분류를 해봅시다",
                correctFeedback = "분류에대한 정답 피드백 ",
                incorrectFeedback = "분류에 대한 오답 피드백",
                sections = new List<Section>{
                    new Section{
                        id = 2,
                        question = "포유류",
                        correctItemIds = new List<int>{
                                    1,
                                    2,
                                    5
                        },
                    },
                    new Section{
                        id = 1,
                        question = "파충류",
                        correctItemIds = new List<int>{
                                    3,
                                    4,
                                    6
                        },
                    },
                },
                sectionItems = new List<SectionItem>{
                        new SectionItem{
                            id = 1,
                            name = "꼬소\n",
                            imageUrl = "https://picsum.photos/200"
                        },
                        new SectionItem{
                            id = 2,
                            name = "호랑이",
                            imageUrl = "https://picsum.photos/id/237/200/300"
                        },
                        new SectionItem{
                            id = 3,
                            name = "사마귀",
                            imageUrl = "https://picsum.photos/seed/picsum/200/300"
                        },
                        new SectionItem{
                            id = 4,
                            name = "바선생 ",
                            imageUrl = "https://picsum.photos/200/300?grayscale\n"
                        },
                        new SectionItem{
                            id = 5,
                            name = "말",
                            imageUrl = "https://picsum.photos/200/300/?blur"
                        },
                        new SectionItem{
                            id = 6,
                            name = "개미",
                            imageUrl = "https://picsum.photos/id/870/200/300?grayscale&blur=2\n"
                        }
                    }
            };

            var solvedQuiz = new ClassificationQuiz
            {
                studentId = 354,
                step = 4,
                question = "포유류",
                categories = new List<string>
                {
                    "포유류",
                    "파충류"
                },
                items = new List<ClassificationItem>
                {
                    new ClassificationItem
                    {
                        name = "꼬소\n",
                        imageUrl = "https://picsum.photos/200"
                    },
                    new ClassificationItem
                    {
                        name = "호랑이",
                        imageUrl = "https://picsum.photos/id/237/200/300"
                    },
                    new ClassificationItem
                    {
                        name = "사마귀",
                        imageUrl = "https://picsum.photos/seed/picsum/200/300"
                    },
                    new ClassificationItem
                    {
                        name = "바선생 ",
                        imageUrl = "https://picsum.photos/200/300?grayscale\n"
                    },
                    new ClassificationItem
                    {
                        name = "말",
                        imageUrl = "https://picsum.photos/200/300/?blur"
                    },
                    new ClassificationItem
                    {
                        name = "개미",
                        imageUrl = "https://picsum.photos/id/870/200/300?grayscale&blur=2\n"
                    }
                },
                activityStage = "EXPERIMENT",
                correctAnswer = new Dictionary<string, List<ClassificationItem>>
                {
                    {
                        "파충류", new List<ClassificationItem>
                        {
                            new ClassificationItem
                            {
                                name = "사마귀",
                                imageUrl = "https://picsum.photos/seed/picsum/200/300"
                            },
                            new ClassificationItem
                            {
                                name = "바선생 ",
                                imageUrl = "https://picsum.photos/200/300?grayscale\n"
                            },
                            new ClassificationItem
                            {
                                name = "개미",
                                imageUrl = "https://picsum.photos/id/870/200/300?grayscale&blur=2\n"
                            }
                        }
                    },
                    {
                        "포유류", new List<ClassificationItem>
                        {
                            new ClassificationItem
                            {
                                name = "꼬소\n",
                                imageUrl = "https://picsum.photos/200"
                            },
                            new ClassificationItem
                            {
                                name = "호랑이",
                                imageUrl = "https://picsum.photos/id/237/200/300"
                            },
                            new ClassificationItem
                            {
                                name = "말",
                                imageUrl = "https://picsum.photos/200/300/?blur"
                            }
                        }
                    }
                },
                id = 5,
                studentAnswer = new Dictionary<string, List<ClassificationItem>>
                {
                    {
                        "파충류", new List<ClassificationItem>
                        {
                            new ClassificationItem
                            {
                                name = "꼬소\n",
                                imageUrl = "https://picsum.photos/200"
                            },
                            new ClassificationItem
                            {
                                name = "호랑이",
                                imageUrl = "https://picsum.photos/id/237/200/300"
                            },
                            new ClassificationItem
                            {
                                name = "사마귀",
                                imageUrl = "https://picsum.photos/seed/picsum/200/300"
                            }
                        }
                    },
                    {
                        "포유류", new List<ClassificationItem>
                        {
                            new ClassificationItem
                            {
                                name = "바선생 ",
                                imageUrl = "https://picsum.photos/200/300?grayscale\n"
                            },
                            new ClassificationItem
                            {
                                name = "말",
                                imageUrl = "https://picsum.photos/200/300/?blur"
                            },
                            new ClassificationItem
                            {
                                name = "개미",
                                imageUrl = "https://picsum.photos/id/870/200/300?grayscale&blur=2\n"
                            }
                        }
                    }
                },
                isCorrect = false,
                correctFeedback = "분류에대한 정답 피드백 ",
                inCorrectFeedback = "분류에 대한 오답 피드백",
                submittedAt = "2025-05-08T07:28:52.299Z",
                timeTaken = 10
            };

            BuildQuiz();
            SetQuizData(quizData);
            // SetSolvedQuizData(solvedQuiz);
            // ShowTeacherView();

        }


    }
}
