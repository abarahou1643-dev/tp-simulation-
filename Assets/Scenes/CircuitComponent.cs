using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircuitComponent : MonoBehaviour
{
    public enum ComponentType { Battery, Resistor, Lamp }

    [Header("Propriétés")]
    public ComponentType componentType;
    public float resistance = 10f;

    [Header("Connexions")]
    public List<CircuitComponent> connectedComponents = new List<CircuitComponent>();

    private bool isDragging = false;
    private Vector3 startPosition;

    void Start()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.allComponents.Add(this);
        }
    }

    void OnMouseDown()
    {
        if (componentType != ComponentType.Battery)
        {
            startPosition = transform.position;
            isDragging = true;
            if (GameManager.Instance != null)
            {
                GameManager.Instance.LogAction("Sélection: " + gameObject.name);
            }
        }
    }

    void OnMouseDrag()
    {
        if (isDragging)
        {
            Vector3 mousePos = GetMouseWorldPosition();
            transform.position = new Vector3(mousePos.x, mousePos.y, transform.position.z);
        }
    }

    void OnMouseUp()
    {
        if (isDragging)
        {
            isDragging = false;
            CheckConnections();
            if (GameManager.Instance != null)
            {
                GameManager.Instance.LogAction("Déplacement: " + gameObject.name);
            }

            if (componentType == ComponentType.Resistor && GameManager.Instance.currentStep == 0)
            {
                GameManager.Instance.CompleteStep(1, 10);
            }
        }
    }

    private Vector3 GetMouseWorldPosition()
    {
        Vector3 mouseScreenPos = Input.mousePosition;
        mouseScreenPos.z = 10f;
        return Camera.main.ScreenToWorldPoint(mouseScreenPos);
    }

    private void CheckConnections()
    {
        connectedComponents.Clear();

        if (GameManager.Instance == null) return;

        foreach (var other in GameManager.Instance.allComponents)
        {
            if (other != this && Vector3.Distance(transform.position, other.transform.position) < 2f)
            {
                connectedComponents.Add(other);
                if (GameManager.Instance != null)
                {
                    GameManager.Instance.LogAction("Connexion: " + name + " ↔ " + other.name);
                }

                if ((componentType == ComponentType.Battery && other.componentType == ComponentType.Resistor) ||
                    (componentType == ComponentType.Resistor && other.componentType == ComponentType.Battery))
                {
                    if (GameManager.Instance.currentStep == 1)
                    {
                        GameManager.Instance.CompleteStep(2, 20);
                    }
                }

                if (componentType == ComponentType.Lamp && GameManager.Instance.currentStep == 2)
                {
                    GameManager.Instance.CompleteStep(3, 20);
                }
            }
        }
    }

    public bool IsConnected()
    {
        return connectedComponents.Count > 0;
    }
}