using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SnapScrollRect : MonoBehaviour, IBeginDragHandler, IEndDragHandler
{
    [SerializeField] private ScrollRect scrollRect;
    [SerializeField] private RectTransform contentPanel;
    [SerializeField] private float snapSpeed = 10f;
    [SerializeField] private float itemSpacing = 100f; // Distance between items
    
    // Delegate for the snap event callback
    public delegate void OnItemCenteredHandler(GameObject centeredObject, int itemIndex);
    
    // Event that other scripts can subscribe to
    public event OnItemCenteredHandler OnItemCentered;
    
    private bool isDragging = false;
    private bool isSnapping = false;
    private int currentIndex = 0;
    private float targetPosition = 0f;
    private float lastDragPosition;
    private float dragDirection;
    
    private void Start()
    {
        if (scrollRect == null)
            scrollRect = GetComponent<ScrollRect>();
            
        if (contentPanel == null)
            contentPanel = scrollRect.content;
    }
    
    private void Update()
    {
        if (!isDragging && !isSnapping)
            return;
            
        if (isSnapping)
        {
            // Smoothly lerp to the target position
            float newX = Mathf.Lerp(contentPanel.anchoredPosition.x, targetPosition, Time.deltaTime * snapSpeed);
            contentPanel.anchoredPosition = new Vector2(newX, contentPanel.anchoredPosition.y);
            
            // Check if we've reached the target position
            if (Mathf.Abs(contentPanel.anchoredPosition.x - targetPosition) < 1.0f)
            {
                contentPanel.anchoredPosition = new Vector2(targetPosition, contentPanel.anchoredPosition.y);
                isSnapping = false;
                
                // Invoke the callback with the centered object
                NotifyCenteredItem();
            }
        }
    }
    
    private void NotifyCenteredItem()
    {
        if (OnItemCentered != null && currentIndex >= 0 && currentIndex < contentPanel.childCount)
        {
            GameObject centeredObject = contentPanel.GetChild(currentIndex).gameObject;
            OnItemCentered.Invoke(centeredObject, currentIndex);
        }
    }
    
    public void OnBeginDrag(PointerEventData eventData)
    {
        isDragging = true;
        isSnapping = false;
        lastDragPosition = eventData.position.x;
    }
    
    public void OnEndDrag(PointerEventData eventData)
    {
        isDragging = false;
        
        // Calculate drag direction (positive = right, negative = left)
        dragDirection = eventData.position.x - lastDragPosition;
        
        SnapToNearestItem();
    }
    
    private void SnapToNearestItem()
    {
        // Calculate the current position
        float currentPos = contentPanel.anchoredPosition.x;
        
        // Calculate raw nearest index based on position
        float rawIndex = currentPos / -itemSpacing;
        
        // Determine the target index based on direction
        int targetIndex;
        
        if (Mathf.Approximately(dragDirection, 0f))
        {
            // If no significant drag, just round to nearest
            targetIndex = Mathf.RoundToInt(rawIndex);
        }
        else
        {
            // If dragging right (positive direction), content moves left (floor)
            // If dragging left (negative direction), content moves right (ceil)
            targetIndex = dragDirection > 0 
                ? Mathf.FloorToInt(rawIndex) 
                : Mathf.CeilToInt(rawIndex);
        }
        
        // Clamp to valid range
        int itemCount = contentPanel.childCount;
        targetIndex = Mathf.Clamp(targetIndex, 0, Mathf.Max(0, itemCount - 1));
        
        // Calculate target position
        targetPosition = -targetIndex * itemSpacing;
        currentIndex = targetIndex;
        
        // Start snapping
        isSnapping = true;
    }
    
    // Public methods for programmatic navigation
    public void GoToNextItem()
    {
        int nextIndex = Mathf.Clamp(currentIndex + 1, 0, contentPanel.childCount - 1);
        SnapToIndex(nextIndex);
    }
    
    public void GoToPreviousItem()
    {
        int prevIndex = Mathf.Clamp(currentIndex - 1, 0, contentPanel.childCount - 1);
        SnapToIndex(prevIndex);
    }
    
    public void SnapToIndex(int index)
    {
        if (isSnapping)
            return;
            
        currentIndex = Mathf.Clamp(index, 0, contentPanel.childCount - 1);
        targetPosition = -currentIndex * itemSpacing;
        isSnapping = true;
    }
}