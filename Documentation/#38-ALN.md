### �������� ������

����� �������� `E_GEN` � `E_KILL` ��� �������� �����

#### ���������� ������

����������� �������� ������ �������� `E_GEN` � `E_KILL` �� �������� �����

#### ����������� ����� � ����� �����

* ������� �����
* ������������ ���
* ���������� ������������ ��������� ��� ����������� ���������

#### ������������� ����� ������

��������� `E_GEN` -- ��� ����� ���������, ������� �������� ��� �������� ��������������� ������� ������ `B`. ������ ���������� �� ��������� �� ���������������� �� ����� �����.

��������� `E_KILL` -- ��� ����� ���������, ������� �������� ��� ��������� �� ��������� ������������ ������� ������ `B`

���� ���������� ��������� `x + y`, ���� �� ��������� `x + y` � ����� �� �������������� `x` � `y`

���� ���������� ��������� `x + y`, ���� �� ����������� `x` ��� `y` � ����� �� ������������� `x + y`

#### ������������ ����� ������ (����������)

* ��� ��������� ����� `TransferFunction`, ������� � ���� ������� ����������� �� ���������� `ITransferFunction`
* ��� ��������� ����� `GetEGenEKill` ��� ������ �������� `e_gen` � `e_kill`
* ��� ���������� ����� `Transfer`, ������� �� ����������� ��� �������� ����� � �������� ��������� `IN` ������� ��������� `e_gen`, `e_kill` � ��������� ������������ ������� ��� ���������� ��������� `OUT`.

<br/><br/>������������ �������<br/><br/>
<img src="images/38/1.png" width="300">

<br/><br/>

�������� ������� �� ���� ������. �� ������ ����� ���������� ����� ��������� `e_gen` ��� �������� �����

```algorithm
	node_index = 0
	foreach node in basicBlock.Nodes:
		if (node is expression as expr):
			redefinition = false
			for next_node in basicBlock.Skip(node_index+1):
				if (next_node is Assign as ass):
					if (ass.Left is Var as lv) && (lv.Id == ass.Result.Id):
						redefinition = true
					if (ass.Right is Var as rv) && (rv.Id == ass.Result.Id):
						redefinition = true
			if !redefinition:
				e_gen.Add(expr.Label)
		node_index++
```

�� ������ ����� ���������� ����� ��������� `e_kill`.
*	������� ������� ������ ���� ���������-���������� �������� �����(�� ����������� ������ `x + y`). 
*	�����, �� ���� ���������-����������(���� `x + y`) ������������ ���� ��������� ���������-���������� �������� �����. 
*	������� ����� ���������-���������� �������� �����.
*	���� ��������� `e_kill` ��� ���� ���������-���������� �������� �������� �����

```algorithm
	foreach node in basicBlock.Nodes:
		if node is Assign as ass:
			basicBlockAssignNodes.Add(ass)

	exceptedAssignNodes = AllAssignNodes.Except(basecBlockAssigns)
	marksBasicBlockAssignNodes = basicBlockAssignNodes.Select(ass_node => ass_node.Result.Id)
	
	foreach ean in exceptedAssignNodes:
		contains = false
		if (ean.Left is Var as lv) && marksBasicBlockAssignNodes.contains(lv.Id):
			contains = true
		if (!contains) && (ean.Right is Var as rv) && marksBasicBlockAssignNodes.contains(rv.Id):
			contains = true
		if contains:
			e_kill.Add(ean.Label)
```

����� `Transfer`

```algorithm
	def Transfer(basicBlock, IN):
		(e_gen, e_kill) = GetEGenEKill(basicBlock)
		outset = Union(IN \ e_kill, e_gen)
```

<!-- #### �����
TODO

#### ������ ������
TODO -->