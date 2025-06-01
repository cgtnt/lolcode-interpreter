## Expressions
<operator> <expression1>
<operator> <expression1> [AN] <expression2>
<operator> <expr1> [[[AN] <expr2>] [AN] <expr3> ...] MKAY

### Math
SUM OF <x> AN <y>       BTW +
DIFF OF <x> AN <y>      BTW -
PRODUKT OF <x> AN <y>   BTW *
QUOSHUNT OF <x> AN <y>  BTW /
MOD OF <x> AN <y>       BTW modulo
BIGGR OF <x> AN <y>     BTW max
SMALLR OF <x> AN <y>    BTW min

<x> and <y> may each be expressions

### Boolean
BOTH OF <x> [AN] <y>          BTW and: WIN iff x=WIN, y=WIN
EITHER OF <x> [AN] <y>        BTW or: FAIL iff x=FAIL, y=FAIL
WON OF <x> [AN] <y>           BTW xor: FAIL if x=y
NOT <x>                       BTW unary negation: WIN if x=FAIL
ALL OF <x> [AN] <y> ... MKAY  BTW infinite arity AND
ANY OF <x> [AN] <y> ... MKAY  BTW infinite arity OR

### Comparison
BOTH SAEM <x> [AN] <y>   BTW WIN iff x == y
DIFFRINT <x> [AN] <y>    BTW WIN iff x != y

BOTH SAEM <x> AN BIGGR OF <x> AN <y>   BTW x >= y
BOTH SAEM <x> AN SMALLR OF <x> AN <y>  BTW x <= y
DIFFRINT <x> AN SMALLR OF <x> AN <y>   BTW x > y
DIFFRINT <x> AN BIGGR OF <x> AN <y>    BTW x < y

### String concatenation 
SMOOSH...MKAY

## Statements
### Variable declaration
I HAS A <variable> ITZ <value>     BTW var declaration
I HAS A <variable>
II HAS A <variable> ITZ A <type>  

<variable> R NOOB   BTW deallocate variable (make it null)

### Variable assignment:> [!WARNING]
I HAS A VAR            BTW VAR is null and untyped
VAR R "THREE"          BTW VAR is now a YARN and equals "THREE"
VAR R 3                BTW VAR is now a NUMBR and equals 3

### Function declaration
HOW DUZ I var YR stuff
	BTW implement
IF U SAY SO

I HAS A var ITZ 0  BTW Throws an error AS var is already taken

var R 0 BTW FUNIKSHUN var no longer exists, it's now NUMBR-0

### Terminal I/O
VISIBLE <expression> [<expression> ...][!]  BTW print
GIMMEH <variable>  BTW input - takes in YARN

### EXPRESSION STATEMENTS 
A bare expression (e.g. a function call or math operation), without any assignment, is a legal statement in LOLCODE. Aside from any side-effects from the expression when evaluated, the final value is placed in the temporary variable IT. IT's value remains in local scope and exists until the next time it is replaced with a bare expression.

### Regular Statements
Have no effect on the IT variable.  

### Control flow


