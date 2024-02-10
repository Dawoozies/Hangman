using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class BoundaryManager : MonoBehaviour
{
    public float boundaryThickness;
    public float boundaryShift;
    public BoxCollider2D boundaryRight;
    public BoxCollider2D boundaryLeft;
    public BoxCollider2D boundaryBottom;
    public BoxCollider2D boundaryTop;

    public float panelShift;
    public RectTransform panelRight;
    public RectTransform panelLeft;
    public RectTransform panelBottom;
    public RectTransform panelTop;
    Camera mainCamera;
    private void Start()
    {
        mainCamera = Camera.main;
        UpdateBoundaryBox();
    }
    void LateUpdate()
    {
        UpdateBoundaryBox();
    }
    void UpdateBoundaryBox()
    {
        float cameraWidth = mainCamera.pixelWidth;
        float cameraHeight = mainCamera.pixelHeight;
        Vector2 boundaryCornerA = (Vector2)mainCamera.ScreenToWorldPoint(Vector2.zero);
        Vector2 boundaryCornerB = (Vector2)mainCamera.ScreenToWorldPoint(new Vector2(cameraWidth, cameraHeight));
        float boundaryLength = Mathf.Abs(boundaryCornerB.x - boundaryCornerA.x);
        float boundaryHeight = Mathf.Abs(boundaryCornerB.y - boundaryCornerA.y);
        boundaryRight.transform.position = new(boundaryCornerB.x - boundaryShift, 0f, 0f);
        boundaryLeft.transform.position = new(boundaryCornerA.x + boundaryShift, 0f, 0f);
        boundaryBottom.transform.position = new(0f, boundaryCornerA.y + boundaryShift, 0f);
        boundaryTop.transform.position = new(0f, boundaryCornerB.y - boundaryShift, 0f);

        boundaryRight.transform.localScale = new(1 + boundaryThickness, boundaryHeight, 1);
        boundaryLeft.transform.localScale = new(1 + boundaryThickness, boundaryHeight, 1);
        boundaryBottom.transform.localScale = new(boundaryLength, 1 + boundaryThickness, 1);
        boundaryTop.transform.localScale = new(boundaryLength, 1 + boundaryThickness, 1);

        Vector2 panelRightWorldPosition = boundaryRight.transform.position + Vector3.right*panelShift;
        Vector2 panelRightScreenPosition = mainCamera.WorldToScreenPoint(panelRightWorldPosition);
        panelRight.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, cameraHeight);
        float panelRightWidth = (cameraWidth - panelRightScreenPosition.x);
        panelRight.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, panelRightWidth);
        panelRight.transform.position = panelRightScreenPosition + new Vector2(panelRightWidth / 2f,0f);

        Vector2 panelLeftWorldPosition = boundaryLeft.transform.position - Vector3.right * panelShift;
        Vector2 panelLeftScreenPosition = mainCamera.WorldToScreenPoint(panelLeftWorldPosition);
        panelLeft.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, cameraHeight);
        float panelLeftWidth = panelLeftScreenPosition.x;
        panelLeft.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, panelLeftWidth);
        panelLeft.transform.position = panelLeftScreenPosition - new Vector2(panelLeftWidth / 2f, 0f);
    }
}
