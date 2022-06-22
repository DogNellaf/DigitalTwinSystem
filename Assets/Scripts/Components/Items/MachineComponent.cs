using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class MachineComponent : ItemComponent
{
    [SerializeField] private TMP_InputField OutputField;
    [SerializeField] private TMP_InputField InputField;
    [SerializeField] private TMP_InputField MaxPowerField;
    [SerializeField] private TMP_InputField SelfPowerField;
    [SerializeField] private TMP_InputField KoefField;

    public string ResourceName;
    public string ProductName;
    public float MaxPower;
    public float SelfPower;
    public float Koef;

    private string inputUnsaved;
    private string outputUnsaved;
    private float maxPowerUnsaved;
    private float selfPowerUnsaved;
    private float koefUnsaved;

    public override void Simulate()
    {
        // �������� ������� �������� ���������������� ����
        var inputs = Inputs.Where(x => x.name == ResourceName);

        // �������� ����� ������������������
        var performance = SelfPower;

        // ���� ������� ����
        if (Worker is not null)
        {
            performance += Worker.GetComponent<WorkerComponent>().Performance;
        }

        if (performance > MaxPower)
        {
            performance = MaxPower;
        }

        float maxValue = 0;

        foreach (var input in inputs)
        {
            maxValue += input.Value;
        }

        // ���� ���� ������� �������� �� ������� �� ������������ ������������������
        if (maxValue <= performance)
        {
            // ���������� ��������� ������������������
            Value += maxValue * (1 / Koef);

            // �������� ��� �������
            foreach (var input in inputs)
            {
                var inputElement = input.gameObject.transform.Find("ConnectionOutput").gameObject;
                var line = GetLine(inputElement, InputConnection);
                line.UpdateText(input.Value);
                input.Value = 0;
            }
        }
        else
        {
            // ������� ���������
            float needed = performance;

            var maxPercent = inputs.Select(x => x.Priority).Sum();

            // ���������� �� ���� ��������
            foreach (var input in inputs)
            {
                var inputElement = input.gameObject.transform.Find("ConnectionOutput").gameObject;

                // ������� �����
                var line = GetLine(inputElement, InputConnection);

                // �������, ����� ����� ������ ���� � ������ ������������
                var part = performance * input.Priority / maxPercent;

                // ���� ������� �� �������� �������
                //if (input.Value > needed)
                //{
                //    Value += needed * (1 / Koef);
                //    input.Value -= needed;
                //    line.UpdateText(needed);
                //    break;
                //}

                // ���� ��������� �������
                if (input.Value > part)
                {
                    // ���������� � �������� ��������
                    Value += part * (1 / Koef);
                    needed -= part * (1 / Koef);
                    input.Value -= part;
                    line.UpdateText(part);
                }

                // ���� �� �������
                else
                {
                    // ���������� ��� ������������������
                    Value += input.Value * (1 / Koef);
                    needed -= input.Value;
                    line.UpdateText(input.Value);
                    input.Value = 0;
                }
            }
        }

        //foreach (var output in Outputs)
        //{
        //    output.Simulate();
        //}
    }

    // Changing input
    public void ChangeInput(string input) => inputUnsaved = input;

    // Changing output
    public void ChangeOutput(string output) => outputUnsaved = output;

    // Changing max power
    public void ChangeMaxPower(string maxPower)
    {
        if (float.TryParse(maxPower, out float result))
        {
            maxPowerUnsaved = result;
        }
        else
        {
            MaxPowerField.text= MaxPower.ToString();
            Debug.LogError("Incorrect Max Power");
        }
    }

    // Changing self power
    public void ChangeSelfPower(string selfPower)
    {
        if (float.TryParse(selfPower, out float result))
        {
            selfPowerUnsaved = result;
        }
        else
        {
            SelfPowerField.text = SelfPower.ToString();
            Debug.LogError("Incorrect Self Power");
        }
    }


    // Changing koef
    public void ChangeKoef(string koefPower)
    {
        if (float.TryParse(koefPower, out float result))
        {
            koefUnsaved = result;
        }
        else
        {
            KoefField.text = Koef.ToString();
            Debug.LogError("Incorrect Koef");
        }
    }

    // Save
    public override void Save()
    {
        ProductName = outputUnsaved;
        ResourceName = inputUnsaved;
        MaxPower = maxPowerUnsaved;
        SelfPower = selfPowerUnsaved;
        Koef = koefUnsaved;
        base.Save();
        UpdateFieldValues();
        DisactivaveUI();
    }

    // Abort
    public override void Abort()
    {
        outputUnsaved = ProductName;
        inputUnsaved = ResourceName;
        maxPowerUnsaved = MaxPower;
        selfPowerUnsaved = SelfPower;
        koefUnsaved = Koef;
        base.Abort();
        UpdateFieldValues();
        DisactivaveUI();
    }



    #region Utils

    // Change fields
    public void UpdateFieldValues()
    {
        OutputField.text = ProductName;
        InputField.text = ResourceName;
        MaxPowerField.text = MaxPower.ToString();
        SelfPowerField.text = SelfPower.ToString();
        KoefField.text = Koef.ToString();
    }

    public override List<string> GetProperties()
    {
        return new List<string> { $"{ResourceName}", $"{ProductName}", $"{MaxPower}", $"{SelfPower}", $"{Priority}", $"{Koef}" };
    }

    #endregion
}
