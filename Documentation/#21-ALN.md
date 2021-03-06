### Название задачи

Представление данных для анализа потока данных (DFA).

#### Постановка задачи

Предоставить инфраструктуру для написания алгоритмов анализа графа потока управления.

#### Зависимости задач в графе задач

* Все оптимизации на уровне CFG

#### Теоретическая часть задачи

Под анализом потоков данных понимают совокупность задач, нацеленных на выяснение некоторых глобальных свойств программы,  то есть извлечение информации о поведении тех или иных конструкциях в некотором контексте. Такая постановка задачи возможна по той причине, что язык программирования и вычислительная среда определяют некоторую общую "безопасную", семантику конструкций, которая годится "на все случаи жизни". Учет же контекстных условий позволяет делать более конкретные, частные заключения о поведении той или иной конструкции;  при этом такие заключения,  вообще говоря,  перестают быть верными в другом контексте.

Например, общая семантика присваивания заключается в вычислении выражения, стоящего в правой части, и присваивании полученного значения в переменную, стоящую в левой части. Однако в случае, когда выражение в правой части не имеет побочных эффектов, а переменная в левой части более нигде не используется, данный оператор становится эквивалентен пустому.  Понятно, что на смысл каждой конструкции может оказывать влияние любая конструкция, из которой в этом графе достижима данная.

Отсюда следует,  что для правильного учета контекста необходимо принять во внимание влияние всех путей до данной вершины: сначала определив влияние каждого пути,  а затем выделив общую часть. Задача осложняется тем, что при наличии циклов множество всех путей в графе управления становится бесконечным.

##### Итерационный алгоритм

Логически процесс решения задачи анализа потоков данных состоит из двух стадий,  выполняемых одновременно.  Локальная стадия заключается в учете влияния отдельного оператора  (группы операторов в узле графа управления)  в предположении,  что уже имеется решение задачи анализа потоков данных перед этим оператором.  На глобальной стадии происходит решение задачи анализа для каждого пути,  ведущего в данную вершину, и затем выделение общей части всех таких решений.

Основной проблемой подхода является проблема остановки алгоритма.  Действительно ,  в какой момент процесс уточнения разметки должен прекратиться? Очевидно, в тот момент, когда получено решение задачи анализа потоков данных. Однако поскольку решение задачи неизвестно, то и воспользоваться этим наблюдением напрямую оказывается невозможным. Поэтому для определения завершаемости алгоритма используется другой принцип -– принцип достижения неподвижонй точки.

Частично-упорядоченное множество X будем называть множеством конечной высоты N тогда и только тогда, когда длины всех строго возрастающих последовательностей элементов X ограничены N. Это означает, что для произвольной возрастающей последовательности начиная с некоторого места все элементы становятся одинаковыми. Рассмотрим теперь функцию перехода F, удовлетворяющую соотношению F(μ) ≥ μ для произвольной разметки μ. Понятно, что при таком условии при итерировании F начиная с некоторого места будет достигнута ее неподвижная точка. Множество X и функция перехода F подбираются таким образом, чтобы эта неподвижная точка являлась решением задачи анализа потоков данных.

#### Практическая часть задачи (реализация)

* Был реализован интерфейс `IAlgorithm`, предками которого обязаны быть все алгоритмы на графе потока управления. Интерфейс содержит единственный метод `Analyze`, который возвращает словарь с парами вида <узел графа потока управления, кортеж `IN` и `OUT` массивов>, полученных в ходе анализа. На вход методу подается тройка <граф потока управления, набор операций сбора алгоритма, передаточная функция>.
* Был реализован интерфейс `ILatticeOperations`, который представляет собой набор операций сбора, определяющих поведение алгоритма. Интерфейс включает:
  * нижнюю и верхнюю границы,
  * бинарный оператор, устанавливающий порядок между двумя элементами `IN`/`OUT`,
  * оператор сбора, который пользователь должен определить сам изходя из потребностей алгоритма.

<!-- #### Тесты
TODO

#### Пример работы.
TODO -->