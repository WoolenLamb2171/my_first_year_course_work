using System;
using System.Collections.Generic;
using System.Linq;

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

public sealed class Node<T>
{
    public T Value { get; private set; }
    public Node<T> Next { get; internal set; }
    public Node<T> Previous { get; internal set; }

    internal Node(T item, Node<T> next, Node<T> previous)
    {
        this.Value = item;
        this.Next = next;
        this.Previous = previous;
    }
}

public sealed class CircularLinkedList<T> : ICollection<T>, IEnumerable<T>
{
    Node<T> head = null;
    Node<T> tail = null;
    int count = 0;
    readonly IEqualityComparer<T> comparer;

    public CircularLinkedList()
        : this(null, EqualityComparer<T>.Default)
    {
    }

    public CircularLinkedList(IEnumerable<T> collection)
        : this(collection, EqualityComparer<T>.Default)
    {
    }

    public CircularLinkedList(IEqualityComparer<T> comparer)
        : this(null, comparer)
    {
    }

    public CircularLinkedList(IEnumerable<T> collection, IEqualityComparer<T> comparer)
    {
        if (comparer == null)
            throw new ArgumentNullException("comparer");
        this.comparer = comparer;
        if (collection != null)
        {
            foreach (T item in collection)
                this.AddLast(item);
            count = collection.Count();
        }
    }

    public Node<T> Tail { get { return tail; } }
    public Node<T> Head
    {
        get { return head; }
        set => head = value;
    }

    public int Count { get { return count; } }

    public Node<T> this[int index]
    {
        get
        {
            if (index >= count || index < 0)
                throw new ArgumentOutOfRangeException("index");
            else
            {
                Node<T> node = this.head;
                for (int i = 0; i < index; i++)
                    node = node.Next;
                return node;
            }
        }
    }

    public void AddLast(T item)
    {
        if (head == null)
            this.AddFirstItem(item);
        else
        {
            Node<T> newNode = new Node<T>(item, head, tail);
            tail.Next = newNode;
            newNode.Next = head;
            newNode.Previous = tail;
            tail = newNode;
            head.Previous = tail;
        }
        ++count;
    }

    void AddFirstItem(T item)
    {
        head = new Node<T>(item, tail, tail);
        tail = head;
        head.Next = tail;
        head.Previous = tail;
    }

    public void AddFirst(T item)
    {
        if (head == null)
            this.AddFirstItem(item);
        else
        {
            Node<T> newNode = new Node<T>(item, head, tail);
            head.Previous = newNode;
            newNode.Previous = tail;
            newNode.Next = head;
            tail.Next = newNode;
            head = newNode;
        }
        ++count;
    }

    public void AddAfter(Node<T> node, T item)
    {
        if (node == null)
            throw new ArgumentNullException("node");
        Node<T> temp = this.FindNode(head, node.Value);
        if (temp != node)
            throw new InvalidOperationException("Node doesn't belong to this list");

        Node<T> newNode = new Node<T>(item, node.Next, node);
        newNode.Next.Previous = newNode;
        node.Next = newNode;

        if (node == tail)
            tail = newNode;
        ++count;
    }

    public void AddAfter(T existingItem, T newItem)
    {
        Node<T> node = this.Find(existingItem);
        if (node == null)
            throw new ArgumentException("existingItem doesn't exist in the list");
        this.AddAfter(node, newItem);
    }

    public void AddBefore(Node<T> node, T item)
    {
        if (node == null)
            throw new ArgumentNullException("node");
        Node<T> temp = this.FindNode(head, node.Value);
        if (temp != node)
            throw new InvalidOperationException("Node doesn't belong to this list");

        Node<T> newNode = new Node<T>(item, node, node.Previous);
        node.Previous.Next = newNode;
        node.Previous = newNode;

        if (node == head)
            head = newNode;
        ++count;
    }

    public void AddBefore(T existingItem, T newItem)
    {
        Node<T> node = this.Find(existingItem);
        if (node == null)
            throw new ArgumentException("existingItem doesn't exist in the list");
        this.AddBefore(node, newItem);
    }

    public Node<T> Find(T item)
    {
        Node<T> node = FindNode(head, item);
        return node;
    }

    public bool Remove(T item)
    {
        Node<T> nodeToRemove = this.Find(item);
        if (nodeToRemove != null)
            return this.RemoveNode(nodeToRemove);
        return false;
    }

    bool RemoveNode(Node<T> nodeToRemove)
    {
        Node<T> previous = nodeToRemove.Previous;
        previous.Next = nodeToRemove.Next;
        nodeToRemove.Next.Previous = nodeToRemove.Previous;

        if (head == nodeToRemove)
            head = nodeToRemove.Next;
        else if (tail == nodeToRemove)
            tail = tail.Previous;

        --count;
        return true;
    }

    public void RemoveAll(T item)
    {
        bool removed = false;
        do
        {
            removed = this.Remove(item);
        } while (removed);
    }

    public void Clear()
    {
        head = null;
        tail = null;
        count = 0;
    }

    public bool RemoveHead()
    {
        return this.RemoveNode(head);
    }

    public bool RemoveTail()
    {
        return this.RemoveNode(tail);
    }

    Node<T> FindNode(Node<T> node, T valueToCompare)
    {
        Node<T> result = null;
        if (comparer.Equals(node.Value, valueToCompare))
            result = node;
        else if (result == null && node.Next != head)
            result = FindNode(node.Next, valueToCompare);
        return result;
    }

    public IEnumerator<T> GetEnumerator()
    {
        Node<T> current = head;
        if (current != null)
        {
            do
            {
                yield return current.Value;
                current = current.Next;
            } while (current != head);
        }
    }

    public IEnumerator<T> GetReverseEnumerator()
    {
        Node<T> current = tail;
        if (current != null)
        {
            do
            {
                yield return current.Value;
                current = current.Previous;
            } while (current != tail);
        }
    }

    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public bool Contains(T item)
    {
        return Find(item) != null;
    }

    public void CopyTo(T[] array, int arrayIndex)
    {
        if (array == null)
            throw new ArgumentNullException("array");
        if (arrayIndex < 0 || arrayIndex > array.Length)
            throw new ArgumentOutOfRangeException("arrayIndex");

        Node<T> node = this.head;
        do
        {
            array[arrayIndex++] = node.Value;
            node = node.Next;
        } while (node != head);
    }

    bool ICollection<T>.IsReadOnly
    {
        get { return false; }
    }

    void ICollection<T>.Add(T item)
    {
        this.AddLast(item);
    }
}

class Program
{
    static void Main(string[] args)
    {
        // Проинициализировал группу
        // var group = new List<Node>()
        // {
        //     new Node(new Student(new int[] { 4, 4, 3, 5, 2 }, "Петров")),
        //     new Node(new Student(new int[] { 5, 5, 5, 4, 5 }, "Сидоров")), //1
        //     new Node(new Student(new int[] { 5, 5, 5, 5, 4 }, "Иванов")), //1
        //     new Node(new Student(new int[] { 3, 3, 3, 2, 2 }, "Смирнов")),
        //     new Node(new Student(new int[] { 5, 5, 4, 5, 2 }, "Васильев")),
        //     new Node(new Student(new int[] { 4, 4, 4, 4, 4 }, "Попов")), //1
        //     new Node(new Student(new int[] { 5, 5, 5, 2, 5 }, "Новиков")),
        //     new Node(new Student(new int[] { 5, 5, 4, 5, 3 }, "Шилович")), //1
        //     new Node(new Student(new int[] { 3, 3, 3, 3, 3 }, "Телегин")), //1
        //     new Node(new Student(new int[] { 4, 4, 3, 4, 4 }, "Никитин")) //1
        // };
        CircularLinkedList<Student> group = new CircularLinkedList<Student>();
        group.AddLast(new Student(new int[] { 4, 4, 2, 5, 4 }, "Петров"));
        group.AddLast(new Student(new int[] { 5, 5, 5, 4, 5 }, "Сидоров"));
        group.AddLast(new Student(new int[] { 5, 5, 5, 5, 4 }, "Иванов"));
        group.AddLast(new Student(new int[] { 3, 3, 3, 2, 2 }, "Смирнов"));
        group.AddLast(new Student(new int[] { 5, 5, 4, 5, 2 }, "Васильев"));
        group.AddLast(new Student(new int[] { 4, 4, 4, 4, 4 }, "Попов"));
        group.AddLast(new Student(new int[] { 5, 5, 5, 2, 5 }, "Новиков"));
        group.AddLast(new Student(new int[] { 5, 5, 4, 2, 3 }, "Шилович"));
        group.AddLast(new Student(new int[] { 3, 3, 2, 3, 5 }, "Телегин"));
        group.AddLast(new Student(new int[] { 4, 4, 3, 4, 5 }, "Никитин"));

        for (int i = 0; i < group.Count; i++)
        {
            var student = group[i];
            foreach (var grade in student.Value.Grades)
            {
                if (grade <= 2)
                {
                    group.Remove(student.Value);
                    i--;
                    break; // выход из внутреннего цикла, так как элемент уже удален
                }
            }
        }
        
        var sortedGroup = group.OrderBy(s => s.AverageGrade);
        
        foreach (var student in group)
        {
            Console.WriteLine($"{student.Name} AVG {student.AverageGrade}");
        }
    }
}
