using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;

namespace Project_Calculator
{
    public partial class Form1 : Form
    {
        private List<string> history = new List<string>(); // Lưu lịch sử phép tính
        private double previousResult = 0; // Lưu giá trị Ans (kết quả lần trước)
        private bool isOperatorClicked = false; // Đánh dấu khi bấm toán tử
        private bool isEqualClicked = false; // Đánh dấu khi bấm dấu "="

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            lbl_old.Text = "";
            lbl_result.Text = "0";
        }

        private void click(object sender, EventArgs e)
        {
            Button btn = sender as Button;

            if (btn == null) return;

            if (char.IsDigit(btn.Text, 0) || btn.Text == ".")
            {
                HandleDigitAndDecimal(btn.Text);
            }
            else if ("+-*/".Contains(btn.Text))
            {
                HandleOperator(btn.Text);
            }
            else if (btn.Text == "=")
            {
                HandleEquals();
            }
            else if (btn.Text == "Ans")
            {
                HandleAns();
            }
            else if (btn.Text == "AC")
            {
                ResetCalculator();
            }
            else if (btn.Text == "<=")
            {
                HandleBackspace();
            }
            else if (btn.Text == "√")
            {
                HandleSquareRoot();
            }
            else if (btn.Text == "^")
            {
                HandlePower();
            }
            else if (btn.Text == "^2")
            {
                HandleSquare();
            }
            else if (btn.Text == "%")
            {
                HandlePercentage();
            }
            else if (btn.Text == "+/-")
            {
                HandleChangeSign();
            }
            else if (btn.Text == "H")
            {
                ShowHistory();
            }
            else if (btn.Text == "(" || btn.Text == ")")
            {
                HandleParenthesis(btn.Text);
            }
            else if (btn.Text == "Exit")
            {
                Application.Exit();
            }
        }

        private void HandleDigitAndDecimal(string text)
        {
            if (lbl_result.Text == "0" || isOperatorClicked || isEqualClicked)
            {
                if (isEqualClicked)
                {
                    lbl_old.Text = "";
                }

                lbl_result.Text = text;
                isEqualClicked = false;
                isOperatorClicked = false;
            }
            else
            {
                lbl_result.Text += text;
            }

            if (!isEqualClicked)
            {
                lbl_old.Text += text;
            }
        }

        private void HandleOperator(string text)
        {
            if (isEqualClicked)
            {
                lbl_old.Text = previousResult.ToString();
                isEqualClicked = false;
            }

            lbl_old.Text += $" {text} ";
            isOperatorClicked = true;
        }

        private void HandleEquals()
        {
            try
            {
                string expression = lbl_old.Text;
                double result = CalculateExpression(expression);

                lbl_old.Text = $"{expression} =";
                lbl_result.Text = result.ToString();

                previousResult = result;
                isOperatorClicked = false;
                isEqualClicked = true;

                history.Add(lbl_old.Text + " " + lbl_result.Text);
            }
            catch
            {
                lbl_result.Text = "Lỗi!";
            }
        }

        private void HandleAns()
        {
            lbl_old.Text += "Ans";
            lbl_result.Text = previousResult.ToString();
        }

        private void ResetCalculator()
        {
            lbl_old.Text = "";
            lbl_result.Text = "0";
            isOperatorClicked = false;
            isEqualClicked = false;
        }

        private void HandleBackspace()
        {
            if (!string.IsNullOrEmpty(lbl_result.Text) && lbl_result.Text != "0")
            {
                // Xóa ký tự cuối cùng trên lbl_result
                lbl_result.Text = lbl_result.Text.Substring(0, lbl_result.Text.Length - 1);

                // Xóa ký tự cuối trên lbl_old (nếu có)
                if (lbl_old.Text.Length > 0)
                {
                    lbl_old.Text = lbl_old.Text.Substring(0, lbl_old.Text.Length - 1);
                }

                // Nếu lbl_result rỗng, đặt lại thành "0"
                if (string.IsNullOrEmpty(lbl_result.Text))
                {
                    lbl_result.Text = "0";
                }
            }
        }

        private void HandleSquareRoot()
        {
            lbl_old.Text += "√(";
        }

        private void HandlePower()
        {
            if (isEqualClicked)
            {
                lbl_old.Text = previousResult.ToString();
                isEqualClicked = false;
            }
            lbl_old.Text += " ^ ";
            isOperatorClicked = true;
        }

        private void HandleSquare()
        {
            try
            {
                double value = double.Parse(lbl_result.Text);
                double result = value * value;
                lbl_old.Text = $"{value}^2 =";
                lbl_result.Text = result.ToString();
                previousResult = result;

                isOperatorClicked = true;
                isEqualClicked = true;
            }
            catch
            {
                lbl_result.Text = "Lỗi!";
            }
        }

        private void HandlePercentage()
        {
            try
            {
                double value = double.Parse(lbl_result.Text);
                double result = value / 100;
                lbl_old.Text = $"{value}% =";
                lbl_result.Text = result.ToString();
                previousResult = result;

                isOperatorClicked = true;
                isEqualClicked = true;
            }
            catch
            {
                lbl_result.Text = "Lỗi!";
            }
        }

        private void HandleChangeSign()
        {
            try
            {
                double value = double.Parse(lbl_result.Text);
                value = -value;

                lbl_result.Text = value.ToString();
                lbl_old.Text = value.ToString();

                previousResult = value;
                isOperatorClicked = true;
                isEqualClicked = false;
            }
            catch
            {
                lbl_result.Text = "Lỗi!";
            }
        }

        private void HandleParenthesis(string text)
        {
            lbl_old.Text += text;
        }

        private double CalculateExpression(string expression)
        {
            expression = expression.Replace("Ans", previousResult.ToString());
            expression = ReplaceSquareRoot(expression); // Xử lý căn bậc hai
            expression = ReplacePower(expression);      // Xử lý số mũ

            try
            {
                DataTable table = new DataTable();
                return Convert.ToDouble(table.Compute(expression, string.Empty));
            }
            catch
            {
                throw new Exception("Lỗi tính toán!");
            }
        }

        private string ReplaceSquareRoot(string expression)
        {
            while (expression.Contains("√("))
            {
                int start = expression.IndexOf("√(");
                int openParentheses = 1;
                int end = start + 2;

                while (openParentheses > 0 && end < expression.Length)
                {
                    if (expression[end] == '(') openParentheses++;
                    else if (expression[end] == ')') openParentheses--;
                    end++;
                }

                if (openParentheses > 0)
                    throw new Exception("Lỗi: Thiếu dấu ngoặc ')'!");

                string inside = expression.Substring(start + 2, end - start - 3);
                double innerValue = CalculateExpression(inside);
                double sqrtResult = Math.Sqrt(innerValue);

                expression = expression.Substring(0, start) + sqrtResult.ToString() + expression.Substring(end);
            }
            return expression;
        }

        private string ReplacePower(string expression)
        {
            while (expression.Contains("^"))
            {
                int index = expression.IndexOf("^");
                string leftPart = expression.Substring(0, index).TrimEnd();
                string rightPart = expression.Substring(index + 1).TrimStart();

                double baseNumber = GetLastNumber(leftPart);
                double exponent = GetFirstNumber(rightPart);

                double result = Math.Pow(baseNumber, exponent);

                expression = expression.Replace($"{baseNumber}^{exponent}", result.ToString());
            }

            return expression;
        }

        private double GetLastNumber(string input)
        {
            string number = new string(input.Reverse().TakeWhile(c => char.IsDigit(c) || c == '.').Reverse().ToArray());
            return double.TryParse(number, out double result) ? result : 0;
        }

        private double GetFirstNumber(string input)
        {
            string number = new string(input.TakeWhile(c => char.IsDigit(c) || c == '.').ToArray());
            return double.TryParse(number, out double result) ? result : 0;
        }

        private void ShowHistory()
        {
            if (history.Count == 0)
            {
                MessageBox.Show("Không có lịch sử!", "Lịch sử", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                string historyContent = string.Join("\n", history);
                MessageBox.Show(historyContent, "Lịch sử", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void lbl_old_Click(object sender, EventArgs e)
        {

        }
    }
}
