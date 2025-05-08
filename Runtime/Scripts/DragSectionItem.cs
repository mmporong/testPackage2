using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace Quiz
{
    public class DragSectionItem : MonoBehaviour, IDragHandler, IPointerDownHandler, IPointerUpHandler
    {
        public int id;
        public string itemName;
        private string imageUrl;

        [SerializeField] private TMP_Text nameText;
        [SerializeField] private Image image;
        private RectTransform rectTransform;
        private Canvas canvas;
        private CanvasGroup canvasGroup;
        private Vector2 originalPosition;
        private Transform originalParent;
        private int originalSiblingIndex;
        private bool isDragging = false;
        private bool isImageLoaded = true;
        public Transform rootParent;
        void Start()
        {
            rectTransform = GetComponent<RectTransform>();
            canvas = GetComponentInParent<Canvas>();
            canvasGroup = GetComponent<CanvasGroup>();
            originalParent = transform.parent;

            if (canvasGroup == null)
            {
                canvasGroup = gameObject.AddComponent<CanvasGroup>();
            }
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            isDragging = true;
            originalPosition = rectTransform.anchoredPosition;
            canvasGroup.blocksRaycasts = false;
            canvasGroup.alpha = 0.6f;
            originalSiblingIndex = transform.GetSiblingIndex();
            transform.SetParent(rootParent);
            transform.SetAsLastSibling();
            if (!isImageLoaded)
            {
                StartCoroutine(LoadImage());
                isImageLoaded = true;
            }
        }


        public void OnDrag(PointerEventData eventData)
        {
            if (isDragging)
            {
                rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
            }
        }

        void IPointerUpHandler.OnPointerUp(PointerEventData eventData)
        {
            bool foundSection = false;
            var results = new System.Collections.Generic.List<RaycastResult>();
            EventSystem.current.RaycastAll(eventData, results);

            foreach (var result in results)
            {
                if (result.gameObject.name == "Section")
                {
                    foundSection = true;
                    // Section 오브젝트를 부모로 설정
                    transform.SetParent(result.gameObject.transform);
                    break;
                }
            }

            // Section 오브젝트를 찾지 못했으면 원래 위치와 부모로 돌아감
            if (!foundSection)
            {
                transform.SetParent(originalParent);
                // rectTransform.anchoredPosition = originalPosition;
                transform.SetSiblingIndex(originalSiblingIndex);
            }

            isDragging = false;
            canvasGroup.blocksRaycasts = true;
            canvasGroup.alpha = 1f;

        }
        public void SetSectionItem(SectionItem sectionItem)
        {
            id = sectionItem.id;
            itemName = sectionItem.name;
            imageUrl = sectionItem.imageUrl;

            nameText.text = itemName;
            StartCoroutine(LoadImage());
        }
        IEnumerator LoadImage()
        {
            // URL로부터 이미지 다운로드 요청
            using (UnityWebRequest request = UnityWebRequestTexture.GetTexture(imageUrl))
            {
                yield return request.SendWebRequest(); // 요청 대기

                // 요청이 성공했는지 확인
                if (request.result == UnityWebRequest.Result.Success)
                {
                    // Texture2D로 변환
                    Texture2D texture = DownloadHandlerTexture.GetContent(request);

                    // Sprite로 변환
                    Sprite sprite = Sprite.Create(
                        texture,
                        new Rect(0, 0, texture.width, texture.height),
                        new Vector2(0.5f, 0.5f)
                    );

                    // Image에 스프라이트 적용
                    image.sprite = sprite;
                }
                else
                {
                    Debug.LogError("이미지를 다운로드하는 데 실패했습니다: " + request.error);
                    isImageLoaded = false;
                }
            }
        }
        void OnDisable()
        {
            isDragging = false;
            if (canvasGroup != null)
            {
                canvasGroup.blocksRaycasts = true;
                canvasGroup.alpha = 1f;
            }
        }

    }
}
