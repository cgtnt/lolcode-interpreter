# Interpeter
An interpreter for a subset of the [LOLCODE esoteric language](http://www.lolcode.org/).

## Language features
- Whitespace is treated as specified in the [original specification](https://github.com/justinmeza/lolcode-spec/blob/master/v1.3/lolcode-spec-v1.3.md#whitespace). However, comment support and the (...) operator are not supported;
- All programs must be opened with the command HAI and closed with KTHXBYE;

### Variables 
- All variable scope is local to the enclosing function or command block;
- Naming and declaration are the same as in the [origianal sepcification](https://github.com/justinmeza/lolcode-spec/blob/master/v1.3/lolcode-spec-v1.3.md#naming);
- Variables are only accessible after declaration;
- Assignment: Assignment of a variable is accomplished with an assignment statement: ```<variable> R <expression>```. The RHS expression must be resolved to a value of matching type of the decalared variable, or any type if the variable is of type NOOB.
- Types are the same as specified in the [original specification](https://github.com/justinmeza/lolcode-spec/blob/master/v1.3/lolcode-spec-v1.3.md#types), however string escape sequences and explicit casting are not supported (only implicit);
- Type NOOB implicitly casts to zero values of each type;
- Deallocation and SRS casting are not supported;

### Operators
- Operators are treated as specificied in the [original specification](https://github.com/justinmeza/lolcode-spec/blob/master/v1.3/lolcode-spec-v1.3.md#operators);

### Terminal I/O
- I/O is treated as specific in the [original specification](https://github.com/justinmeza/lolcode-spec/blob/master/v1.3/lolcode-spec-v1.3.md#inputoutput);

### Loops
Loops have the following form:
```
IM IN YR <variable declaration>, WILE <expression>
  <code block>
IM OUTTA YR 
```
Where ```<variable declaration>``` is a variable declaration statement, with TYPE or VALUE of variable specified. The variable will be declared in the local scope of the loop. 

### Functions
