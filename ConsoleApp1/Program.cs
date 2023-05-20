using System;
using System.Linq;
using System.Collections.Generic;

public class Student
{
    public double AverageGrade = 0;
    public string Name;
    public int MinGrade = 1000;
    public int[] Grades;

    public Student()
    {
    }

    public Student(int[] grades, string name)
    {
        Grades = grades;
        Name = name;
        for (int i = 0; i < Grades.Length; i++)
        {
            AverageGrade += Grades[i];
            if (Grades[i] < MinGrade)
            {
                MinGrade = Grades[i];
            }
        }
        AverageGrade = AverageGrade / grades.Length;
    }
}

public class StudentWithBonus : Student
{
    public int Bonus;

    public StudentWithBonus(int bonus, int[] grades, string name) : base(grades, name)
    {
        Bonus = bonus;
    }
}

public class Node
{
    public Student Student;
    public StudentWithBonus StudentWithBonus;
    public Node NextNode;
    public Node PreviousNode;

    public Node(Student student)
    {
        Student = student;
    }

    public Node GetNext()
    {
        return NextNode;
    }

    public Node GetPrevious()
    {
        return PreviousNode;
    }
}

class Program
{
    static void Main(string[] args)
    {
        // Проинициализировал группу
        var group = new List<Node>()
        {
            new Node(new Student(new int[] { 4, 4, 3, 5, 2 }, "Петров")),
            new Node(new Student(new int[] { 5, 5, 5, 4, 5 }, "Сидоров")), //1
            new Node(new Student(new int[] { 5, 5, 5, 5, 4 }, "Иванов")), //1
            new Node(new Student(new int[] { 3, 3, 3, 2, 2 }, "Смирнов")),
            new Node(new Student(new int[] { 5, 5, 4, 5, 2 }, "Васильев")),
            new Node(new Student(new int[] { 4, 4, 4, 4, 4 }, "Попов")), //1
            new Node(new Student(new int[] { 5, 5, 5, 2, 5 }, "Новиков")),
            new Node(new Student(new int[] { 5, 5, 4, 5, 3 }, "Шилович")), //1
            new Node(new Student(new int[] { 3, 3, 3, 3, 3 }, "Телегин")), //1
            new Node(new Student(new int[] { 4, 4, 3, 4, 4 }, "Никитин")) //1
        };

        // Удаляем двоечников
        group = group.Where(node => !node.Student.Grades.Contains(2)).ToList();

        // Сортируем узлы по среднему баллу студента
        for (var i = 0; i < group.Count; i++)
        {
            for (var j = 0; j < group.Count - 1; j++)
            {
                if (group[j].Student.AverageGrade < group[j + 1].Student.AverageGrade)
                {
                    var temp = group[j];
                    group[j] = group[j + 1];
                    group[j + 1] = temp;
                }
            }
        }

        // Связываем узлы
        for (var i = 0; i < group.Count; i++)
        {
            if (i == 0)
            {
                group[i].PreviousNode = group[group.Count - 1];
                group[i].NextNode = group[i + 1];
            }
            else if (i == (group.Count - 1))
            {
                group[i].PreviousNode = group[i - 1];
                group[i].NextNode = group[0];
            }
            else
            {
                group[i].PreviousNode = group[i - 1];
                group[i].NextNode = group[i + 1];
            }
        }

        foreach (var student in group)
        {
            Console.WriteLine($"{student.Student.Name} - {student.Student.AverageGrade}");
        }
    }
}