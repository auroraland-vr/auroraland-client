using UnityEngine;
using UnityEngine.UI;

namespace Auroraland
{
    public class ScrollRectSetter : MonoBehaviour
    {
        private ScrollRect scrollRect;
        private Transform content;

        // Use this for initialization
        void Start()
        {
            scrollRect = GetComponent<ScrollRect>();
            scrollRect.verticalNormalizedPosition = 0;
            content = scrollRect.content;
        }

        public void SetVerticalScrollRectPosition(int index, int totalCount, float buttonHeight)
        {
            Vector2 position = content.GetComponent<RectTransform>().anchoredPosition;
            VerticalLayoutGroup layoutGroup = content.GetComponent<VerticalLayoutGroup>();
            float spacing = layoutGroup.spacing;
            float maskHeight = scrollRect.GetComponent<RectTransform>().rect.height; //height of shown panel
            float contentHeight = content.GetComponent<RectTransform>().rect.height; //the full list of content height
            int numOfRows = Mathf.CeilToInt(maskHeight / (buttonHeight + spacing)); // the total number of rows of the panel

            if (index == 0) //reset to front
            {
                position.y = 0;
            }
            else if (index + numOfRows == totalCount) //reset the first item of the last page to be on top of list (not cut in half)
            {
                position.y = contentHeight - (buttonHeight + spacing) * numOfRows;
            }
            else if (index + numOfRows > totalCount) //reset to end, don't move the scroll rect
            {
                position.y = contentHeight - maskHeight;
            }
            else
            {
                position.y = index * (buttonHeight + spacing);
            }
            content.GetComponent<RectTransform>().anchoredPosition = position;

        }

        public void SetVerticalScrollGridRectPosition(int index, int totalCount, float buttonHeight)
        {
            GridLayoutGroup layoutGroup = content.GetComponent<GridLayoutGroup>();
            Vector2 position = content.GetComponent<RectTransform>().anchoredPosition;
            float spacing = layoutGroup.spacing.y;
            float maskWidth = scrollRect.GetComponent<RectTransform>().rect.width;
            float maskHeight = scrollRect.GetComponent<RectTransform>().rect.height; //height of shown panel
            float contentHeight = content.GetComponent<RectTransform>().rect.height; //the full list of content height
            int itemsPerRow = Mathf.FloorToInt(maskWidth / (layoutGroup.cellSize.x + spacing)); //how many cells in this row
            int rowsPerPage = Mathf.FloorToInt(maskHeight / (buttonHeight + spacing)); // the total number of rows of the panel
            float totalRows = Mathf.CeilToInt(totalCount / (float)itemsPerRow);
            float currentRowIndex = Mathf.FloorToInt(index / (float)itemsPerRow);

            if (index == 0) //reset to front
            {
                position.y = 0;
            }
            else if ((currentRowIndex + rowsPerPage) >= totalRows) //reset to end
            {
                position.y = contentHeight - maskHeight;
            }
            else
            {
                position.y = currentRowIndex * (buttonHeight + spacing);
            }

            content.GetComponent<RectTransform>().anchoredPosition = position;
        }

        public void SetHorizontalScrollRectPosition(int index, int totalCount, float buttonWidth, int extra = 0)
        {
            Vector2 position = content.GetComponent<RectTransform>().anchoredPosition;
            float maskWidth = scrollRect.GetComponent<RectTransform>().rect.width;
            float contentWidth = content.GetComponent<RectTransform>().rect.width;
            HorizontalLayoutGroup layoutGroup = content.GetComponent<HorizontalLayoutGroup>();
            int itemsPerRow = Mathf.CeilToInt(maskWidth / (buttonWidth + layoutGroup.spacing));

            switch (index)
            {
                //reset to front
                case 0:
                    position.x = 0;
                    break;
                case 1: //we want the default selected space be always at the left, and this is for list like my space that has extra create button that is not counted
                    position.x = (buttonWidth + layoutGroup.spacing) * (extra + index) + layoutGroup.padding.left;
                    break;
            }

            if (index + itemsPerRow == totalCount) //reset the first item of the last page to be the first of list (not cut in half)
            {
                //Debug.LogFormat("index:{0}, pos:{1}", index, -(contentWidth - (buttonWidth + layoutGroup.spacing) * (itemsPerRow) - layoutGroup.padding.right));
                position.x = contentWidth - (buttonWidth + layoutGroup.spacing) * (itemsPerRow) - layoutGroup.padding.right;
            }
            else if (index + itemsPerRow > totalCount)
            {
                //Debug.LogFormat("index:{0}, pos:{1}", index, -(contentWidth - maskWidth));
                position.x = contentWidth - maskWidth;
            }
            else
            {
                position.x = index * (buttonWidth + layoutGroup.spacing);
            }
            content.GetComponent<RectTransform>().anchoredPosition = -position;
        }
    }
}