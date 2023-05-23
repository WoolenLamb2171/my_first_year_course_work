using System.Collections;

public class Student
{
    public double AverageGrade;
    public string Name;
    public int MinGrade = 1000;
    public int[] Grades;

    public Student(int[] grades, string name)
    {
        Grades = grades;
        Name = name;
        foreach (var t in Grades)
        {
            AverageGrade += t;
            if (t < MinGrade)
            {
                MinGrade = t;
            }
        }

        AverageGrade /= grades.Length;
    }
}

public class StudentWithBonus : Student
{
    public int Bonus;

    public StudentWithBonus(int[] grades, string name, int bonus) : base(grades, name)
    {
        Bonus = bonus;
    }
}

public class Node<T>
{
    public T Value { get; }
    public Node<T>? Next { get; internal set; }
    public Node<T>? Previous { get; internal set; }

    internal Node(T item, Node<T>? next, Node<T>? previous)
    {
        Value = item;
        Next = next;
        Previous = previous;
    }
}

public sealed class CircularLinkedList<T> : ICollection<T>
{
    private readonly IEqualityComparer<T> _comparer;

    public Node<T>? Tail { get; private set; }
    public Node<T>? Head { get; private set; }
    public int Count { get; private set; }
    
    public CircularLinkedList(IEnumerable<T>? collection, IEqualityComparer<T> comparer)
    {
        _comparer = comparer;
        if (collection != null)
        {
            var enumerable = collection.ToList();
            foreach (var item in enumerable)
                AddToTail(item);
            Count = enumerable.Count();
        }
    }
    
    public CircularLinkedList() : this(null, EqualityComparer<T>.Default) { }

    public Node<T> this[int index]
    {
        get
        {
            if (index >= Count || index < 0)
                throw new ArgumentOutOfRangeException(nameof(index));
            

            Node<T> node = Head ?? throw new InvalidOperationException();
            for (int i = 0; i < index; i++)
                node = node!.Next!;
            return node;
        }
    }

    public void AddToTail(T item)
    {
        if (Head == null || Tail == null)
        {
            AddFirstItem(item);
        }
        else
        {
            Node<T> newNode = new Node<T>(item, Head, Tail);
            Tail.Next = newNode;
            newNode.Next = Head;
            newNode.Previous = Tail;
            Head.Previous = newNode;
            Tail = newNode;
        }

        ++Count;
    }
    
    public void AddToHead(T item)
    {
        if (Head == null || Tail == null)
            AddFirstItem(item);
        else
        {
            Node<T> newNode = new Node<T>(item, Head, Tail);
            Head.Previous = newNode;
            newNode.Next = Head;
            newNode.Previous = Tail;
            Tail.Next = newNode;
            Head = newNode;
        }

        ++Count;
    }

    private void AddFirstItem(T item)
    {
        Head = new Node<T>(item, null, null);
        Tail = Head;
        Head.Next = Tail;
        Head.Previous = Tail;
    }
    

    public void AddAfter(Node<T> node, T item)
    {
        if (node == null || node.Next == null)
            throw new ArgumentNullException(nameof(node));
        if (Head == null)
            throw new NullReferenceException(nameof(Head));
        if (FindNode(Head, node.Value) != node)
            throw new InvalidOperationException("Node doesn't belong to this list");

        Node<T> newNode = new Node<T>(item, node.Next, node);
        newNode.Next!.Previous = newNode;
        node.Next = newNode;

        if (node == Tail)
            Tail = newNode;
        ++Count;
    }

    public void AddAfter(T existingItem, T newItem)
    {
        Node<T>? node = Find(existingItem);
        if (node == null)
            throw new ArgumentException("existingItem doesn't exist in the list");
        AddAfter(node, newItem);
    }

    public void AddBefore(Node<T> node, T item)
    {
        if (node == null || node.Previous == null)
            throw new ArgumentNullException(nameof(node));
        if (Head == null)
            throw new NullReferenceException(nameof(Head));
        Node<T>? temp = FindNode(Head, node.Value);
        if (temp != node)
            throw new InvalidOperationException("Node doesn't belong to this list");

        Node<T> newNode = new Node<T>(item, node, node.Previous);
        node.Previous.Next = newNode;
        node.Previous = newNode;

        if (node == Head)
            Head = newNode;
        ++Count;
    }

    public void AddBefore(T existingItem, T newItem)
    {
        Node<T>? node = Find(existingItem);
        if (node == null)
            throw new ArgumentException("existingItem doesn't exist in the list");
        AddBefore(node, newItem);
    }

    public Node<T>? Find(T item)
    {
        return FindNode(Head ?? throw new InvalidOperationException(), item);
    }

    public bool Remove(T item)
    {
        Node<T>? nodeToRemove = Find(item);
        if (nodeToRemove != null)
            return RemoveNode(nodeToRemove);
        return false;
    }
    
    private bool RemoveNode(Node<T>? nodeToRemove)
    {
        if (nodeToRemove == null || nodeToRemove.Previous == null || nodeToRemove.Next == null)
            throw new ArgumentNullException(nameof(nodeToRemove));
        
        Node<T> previous = nodeToRemove.Previous;
        previous.Next = nodeToRemove.Next;
        nodeToRemove.Next.Previous = nodeToRemove.Previous;

        if (Head == nodeToRemove)
            Head = nodeToRemove.Next;
        else if (Tail == nodeToRemove)
            Tail = Tail.Previous;

        --Count;
        return true;
    }

    public void RemoveAll(T item)
    {
        bool removed = false;
        do
        {
            removed = Remove(item);
        } while (removed);
    }

    public void Clear()
    {
        Head = null;
        Tail = null;
        Count = 0;
    }

    public bool RemoveHead()
    {
        return RemoveNode(Head);
    }

    public bool RemoveTail()
    {
        return RemoveNode(Tail);
    }

    private Node<T>? FindNode(Node<T> node, T valueToCompare)
    {
        Node<T>? result = null;
        if (_comparer.Equals(node.Value, valueToCompare))
            result = node;
        else if (result == null && node.Next != Head)
            result = FindNode(node.Next!, valueToCompare);
        return result;
    }

    public IEnumerator<T> GetEnumerator()
    {
        if (Head == null)
            throw new NullReferenceException(nameof(Head));
        Node<T> current = Head;
        if (current != null)
        {
            do
            {
                yield return current!.Value;
                current = current.Next!;
            } while (current != Head);
        }
    }

    public IEnumerator<T> GetReverseEnumerator()
    {
        if (Tail == null)
            throw new NullReferenceException(nameof(Head));
        Node<T> current = Tail;
        if (current != null)
        {
            do
            {
                yield return current!.Value;
                current = current.Previous!;
            } while (current != Tail);
        }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public bool Contains(T item)
    {
        return Find(item) is not null;
    }

    public void CopyTo(T[] array, int arrayIndex)
    {
        if (array == null)
            throw new ArgumentNullException(nameof(array));
        if (arrayIndex < 0 || arrayIndex > array.Length)
            throw new ArgumentOutOfRangeException(nameof(arrayIndex));

        Node<T> node = Head ?? throw new NullReferenceException(nameof(Head));
        do
        {
            array[arrayIndex++] = node!.Value;
            node = node.Next!;
        } while (node != Head);
    }

    bool ICollection<T>.IsReadOnly => false;

    void ICollection<T>.Add(T item)
    {
        AddToTail(item);
    }
}

internal static class Program
{
    private static void Main(string[] args)
    {
        CircularLinkedList<Student> group = new CircularLinkedList<Student>();
        group.AddToTail(new Student(new[] { 4, 4, 2, 5, 4 }, "Петров"));
        group.AddToTail(new StudentWithBonus(new[] { 5, 5, 5, 4, 5 }, "Сидоров", 2000));
        group.AddToTail(new Student(new[] { 5, 5, 5, 5, 4 }, "Иванов"));
        group.AddToTail(new Student(new[] { 3, 3, 3, 2, 2 }, "Смирнов"));
        group.AddToHead(new StudentWithBonus(new[] { 5, 5, 4, 5, 2 }, "Васильев", 1000));
        group.AddToHead(new Student(new[] { 4, 4, 4, 4, 4 }, "Попов"));
        group.AddToTail(new StudentWithBonus(new[] { 5, 5, 5, 2, 5 }, "Новиков", 2000));
        group.AddToTail(new Student(new[] { 5, 5, 4, 2, 3 }, "Шилович"));
        group.AddToTail(new Student(new[] { 3, 3, 2, 3, 5 }, "Телегин"));
        group.AddToTail(new Student(new[] { 4, 4, 3, 4, 5 }, "Никитин"));

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

        var sortedGroup = group.OrderByDescending(s => s.AverageGrade);

        foreach (var student in sortedGroup)
        {
            if (student is StudentWithBonus bonusSt)
            {
                Console.WriteLine($"{bonusSt.Name} — AVG: {bonusSt.AverageGrade} — Bonus: {bonusSt.Bonus}");
            }
            Console.WriteLine($"{student.Name} — AVG {student.AverageGrade}");
        }
    }
}