# Interpeter
An interpreter for a subset of the [LOLCODE esoteric language](http://www.lolcode.org/).
Usage:
```
[executable] <LOLCODE source file>
```

## Language features
- Whitespace is treated as specified in the [original specification](https://github.com/justinmeza/lolcode-spec/blob/master/v1.3/lolcode-spec-v1.3.md#whitespace). However, comment support and the (...) operator are not supported;
- All programs must be opened with the command HAI and closed with KTHXBYE. HAI should NOT be followed by a version number, contrary to the original specification;

### Variables 
- All variable scope is local to the enclosing function or command block;
- Naming and declaration are the same as in the [origianal sepcification](https://github.com/justinmeza/lolcode-spec/blob/master/v1.3/lolcode-spec-v1.3.md#naming);
- Variables are only accessible after declaration;
- Assignment: Assignment of a variable is accomplished with an assignment statement: ```<variable> R <expression>```. The RHS expression must be resolved to a value of matching type of the decalared variable, or any type if the variable is of type NOOB.
- Types are the same as specified in the [original specification](https://github.com/justinmeza/lolcode-spec/blob/master/v1.3/lolcode-spec-v1.3.md#types), however string escape sequences and explicit casting are not supported (only implicit);
- Type NOOB implicitly casts to zero values of each type;
- Deallocation, arrays and SRS casting are not supported;

### Operators
- Operators are treated as specificied in the [original specification](https://github.com/justinmeza/lolcode-spec/blob/master/v1.3/lolcode-spec-v1.3.md#operators);

### Terminal I/O
- I/O is treated as specified in the [original specification](https://github.com/justinmeza/lolcode-spec/blob/master/v1.3/lolcode-spec-v1.3.md#inputoutput);
- The print (to STDOUT or the terminal) operator is VISIBLE. It has infinite arity and implicitly concatenates all of its arguments after casting them to YARNs. It is terminated by the statement delimiter (line end or comma). The output is followed by a newline if the final token is terminated with an exclamation point (!).

### Loops
Loops have the following form:
```
IM IN YR <variable declaration>, WILE <expression>
  <code block>
IM OUTTA YR 
```
Where ```<variable declaration>``` is a variable declaration statement, with TYPE or VALUE of variable specified. The variable will be declared in the local scope of the loop. 

### Functions
Function definition and calls are treated as specified in the [original specification](https://github.com/justinmeza/lolcode-spec/blob/master/v1.3/lolcode-spec-v1.3.md#functions).
