using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

#nullable enable


public class ItemComponent : Element
{
    public ItemType Type;
    public WorkerComponent Worker;
    public List<ItemComponent> Inputs = new();
    public float Value;

    [SerializeField] protected GameObject InputConnection;
    [SerializeField] protected GameObject OutputConnection;
    [SerializeField] protected GameObject UI;
    [SerializeField] protected TMP_InputField NameField;
    [SerializeField] protected string NameUnsaved;

    protected WorkspaceModel Model => TwinApplication.GetModel<WorkspaceModel>();
    protected string? InputName
    {
        get
        {
            try
            {
                return InputConnection.name;
            }
            catch
            {
                return null;
            }
        }
    }
    protected string? OutputName
    {
        get
        {
            try
            {
                return OutputConnection.name;
            }
            catch
            {
                return null;
            }
        }
    }

    // Start function
    public override void Start()
    {
        if (NameField != null)
        {
            NameField.text = gameObject.name;
            NameUnsaved = gameObject.name;
        }
    }

    public virtual void Step()
    {

    }

    // Add input in the list
    public void AddInput(ItemComponent item)
    {
        Inputs.Add(item);
        Debug.Log($"����� ����� {item.name} � {name} �������");

        var start = item.transform.Find("ConnectionOutput").transform.position;
        var end = InputConnection.transform.position;
        DrawLine(start, end, transform);
    }

    // When User click
    public override void OnMouseDown()
    {
        Ray ray = TwinApplication.Camera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            string hitObjectName = hit.collider.gameObject.name;
            if (hitObjectName == name)
            {
                UI.SetActive(true);
            }
            else if (hitObjectName == InputName)
            {
                var inputObject = Model.SelectedOutputConnection;
                var connection = inputObject.GetComponent<ItemComponent>();
                if (connection is not null)
                {
                    if (connection.gameObject != gameObject)
                    {
                        if (!Inputs.Contains(connection))
                        {
                            AddInput(connection);
                            Model.SelectedOutputConnection = null;
                        }
                        else
                        {
                            Debug.LogError("����� ���������� ��� ����");
                        }
                    }
                    else
                    {
                        Debug.LogError("�������� ��������� ������ ���������");
                    }
                }
                else
                {
                    Debug.Log("������ �������� �������������� ����������");
                }
            }
            else if (hitObjectName == OutputName)
            {
                TwinApplication.GetModel<WorkspaceModel>().SelectedOutputConnection = gameObject;
                Debug.Log($"�������������� ���������� � ������� {name} �������");
            }
        }
    }

    // Disactive panel
    public void DisactivaveUI()
    {
        UI.SetActive(false);
    }

    // Delete the item
    public void Delete() => Destroy(gameObject);

    // Changing name
    public void ChangeName(string name) => NameUnsaved = name;

    // Save
    public virtual void Save()
    {
        gameObject.name = NameUnsaved;
        NameField.text = gameObject.name;
        DisactivaveUI();
    }

    // Abort
    public virtual void Abort()
    {
        NameUnsaved = gameObject.name;
        NameField.text = NameUnsaved;
        DisactivaveUI();
    }

    public static void DrawLine(Vector3 start, Vector3 end, Transform transform)
    {
        GameObject line = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        var p3 = (start + end) * 0.5f;
        line.transform.parent = transform.parent;
        line.transform.localPosition = p3;
        line.transform.rotation = Quaternion.Euler(0, -90, 0);
        line.transform.localScale = new Vector3(0.01f, 0.01f, Vector3.Distance(start, end));
        line.transform.position = p3; //placebond here
        line.transform.LookAt(end);
        line.layer = LayerMask.NameToLayer("Ignore Raycast");
        line.name = $"line |{start}| to |{end}|";
    }
    
    public virtual Dictionary<string, string> GetProperties()
    {
        throw new System.Exception();
    }
}
