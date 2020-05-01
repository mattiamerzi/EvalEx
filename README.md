![GitHub](https://img.shields.io/github/license/mattiamerzi/EvalEx)
[![Build Status](https://travis-ci.com/mattiamerzi/EvalEx.svg?branch=master)](https://travis-ci.com/mattiamerzi/EvalEx)
![GitHub issues](https://img.shields.io/github/issues-raw/mattiamerzi/EvalEx)

# EvalEx -- Expression Evaluator

EvalEx is a simple yet powerful expression evaluator written in C# and provided as a class library. EvalEx extensively supports `double`-based calculations, and has a limited support for `double[]`, `string`, integer (as `long` data types) and JSON objects, using the Newtonsoft library.

EvalEx can be extended adding new functions and even operators.
This project initially started porting to C# the EvalEx Java project by Udo Klimaschewski (thanks 🙏).

## Basic Usage
```
using EvalEx;

Expression e = new Expression();
e.SetIntVariable("A", 10);
e.SetIntVariable("a", 20);
long result = e.EvalInt("a+A"); // -> 30
```
## Installation
You can choose to install from source, downloading the solution from the repository, or to install via NuGet
```
Install-Package EvalEx
```
## Variables
Basic variables data types are `double`, `double[]` and `string`. JSON objects are basically handled as strings, and converted to JSON only when a JSON properties or JSON Path expressions are used.

Variables are added to expression instances and a name is assigned. Note that variable names are case sensitive.
```
using EvalEx;

Expression e = new Expression();
e.SetDoubleVariable("d", 123.45d);
e.SetArrayVariable("a", new double[] { 12.1d, 23.2d, 34.3d });
e.SetStringVariable("s", "Hello World!");
e.SetStringVariable("s", "{ \"key\" : 123 }");
```

## Functions
Functions can be added as C# classes, extending the `Function` or `LazyFunction` (for lazily evaluated functions) class
```
protected class TestSum : Function
{
    public TestSum() : base("TESTSUM", 2) { }

    public override double Eval(List<double> parameters)
    {
        return parameters[0] + parameters[1];
    }
}
...
e.AddFunction(new TestSum());
```
or using the `DynaFunction` class, using a specific syntax
```
FUNCTION(param1: [type], param2: [type], ...): [return type] => [function]
```
`[type]` can be one of Float, Int, String.
```
DynaFunction d = new DynaFunction("PITAGORA(leg1: Float, leg2: Float): Float => SQRT( leg1^2 + leg2^2 )");
expression.AddLazyFunction(d.AsLazyFunction());
```

## Operators
Custom operators can be added extending the `Operator` class, or using one of the helper classes, like `FunOperator`, `FunStringOperator`, `FunBoolOperator`, that helps you define new operators using only a simple Lambda expression. For example, to add a `|` operator that concatenates two strings you can define:
```
e.AddOperator(new FunStringOperator("|", 20, true, (v1, v2) => v1 + v2));
```

## Expression Break
Sometimes, you may want to inform your application that a particular expression should not return a value. In such cases, you can use the `IFBRK(check, result)` function. This function returns `result` if `check` returns a true (!= 0) value, otherwise throws an `ExpressionBreakException`
```
e.EvalDouble("IFBRK(3<5, 1.0)"); // returns 1.0 (double
e.EvalDouble("IFBRK(3>5, 1.0)"); // throws ExpressionBreakException
```

## Arrays, map and reduce
EvalEx offers a basic array support (see "Builtin Functions" for more informations), and depending on the use cases, you'll probably want to write your own functions to handle with them. Nevertheless, there are two operators to work with arrays: map (`$`) and reduce (`@`).

Map iteratively invokes a function that takes at least one parameter on any element of the array and returns a "mutated" version of the array.

Reduce invokes a function that takes at least two parameters passing the first two elements of the array as arguments, and then iteratively invokes the same functions passing as first argument the result of the preceding computation, and the next element of the array as second argument.

Suppose you have declared in your expression two functions "SQUARE(x)" defined as `x^2`, and "SUM(a,b)" defined as `a+b`.
```
double[] arr = new double[] { 1d, 3d, 5d };
expression.EvalArray("$SQUARE(arr)")); // returns { 1d, 9d, 25d }
expression.EvalInt("@SUM(arr)")); // returns (1+3)+5 => 9
```

## CachingExpression
If you plan to evaluate the same function in a loop, and the function might be computational intensive, you can use the `CachingExpression` class, superclass of `Expression`, that caches a _signature_ of the expression and returns a cached value of it.

The signature is made by the _functional_ tokens of the expression, that means that whitespaces and other non-functional characters are ignored, and the value of the variables, that means that if you change the value of a variable the signature changes.

Avoid caching if you use non-deterministic functions, such as `RANDOM()`, because you would receive always the same result, and this surely is NOT the expected behaviour!

## Expressions showcase
### Simple numeric expressions
```
"5+3" -> 8
"SIN(PI)" -> 0
"x^3" -> 8 (x = 2)
"3^x" -> 9
"81^0.75" -> 27
```

### Implicit Multiplication
Implicit multiplication is evil, and should be avoided.
```
e.SetIntVariable("a", 5);
e.SetIntVariable("b", 3);
e.SetIntVariable("c", 3);
e.EvalInt("3(5)")); // 15
e.EvalInt("(3)(5)")); // 15
e.EvalInt("3(a)")); // 15
e.EvalInt("(a)b")); // 15
e.EvalInt("(a+b)c")); // 24
e.EvalInt("10(a+b)")); // 80
e.EvalInt("(a b c)")); // 45
e.EvalInt("(a 2 b 3 c)")); // 270
```

### IF
`IF` function is always lazily evaluated
```
IF(3<5, 1, 3/0); // returns 1, does not throw a division by zero
```


### Scientific Notation
Supported with both uppercase or lowercase `E`.
```
e.EvalInt("123.4567E4"); /// 1234567 (long)
e.EvalDouble("1e-10"); // 0.0000000001d (double)
```

### JSON
Simple JSON properties access can be inlined using dot notation
```
e.SetStringVariable("jj", @"{'adouble':123.45d,'aint':37, 'anested': { 'nstring': 'hello world' } }");
e.EvalInt("jj.aint"); // 37
e.EvalString("jj.nested.nstring"); // "hello world"
```
more complex JSON Path expressions can be put inside double quotes:
```
e.SetStringVariable("bigjson", @"{
  'Stores': [
    'Lambton Quay',
    'Willis Street'
  ],
  'Manufacturers': [
    {
      'Name': 'Acme Co',
      'Products': [
        {
          'ID': 123,
		  'Deps': [ 1, 3, 5 ],
          'Name': 'Anvil',
          'Price': 50
        }
      ]
    },
    {
      'Name': 'Contoso',
      'Products': [
        {
          'ID': 737,
		  'Deps': [ 2, 4, 6 ],
          'Name': 'Elbow Grease',
          'Price': 99.95
        },
        {
          'ID': 999,
		  'Deps': [ 7, 8, 9 ],
          'Name': 'Headlight Fluid',
          'Price': 4
        }
      ]
    }
  ]
}");

expression.EvalInt("bigjson.\"$.Manufacturers[?(@.Name == 'Acme Co')].Products[0].Price\"")); // 50

// if multiple values are returned, double or double[] values, they will be merged:
expression.EvalArray("bigjson.\"$..Products[?(@.Price >= 50)].Deps\"")); //  { 1, 3, 5, 2, 4, 6 }
```
## Builtins
### Operators
#### Double-based Operators
Arithmetic operators `+`, `-`, `*`, `/`, modulus `%`, and power `^` operator.

Unary `+` and `-` operators.

Logical operators and `&&`, or `||` and not `!`.

Equal to `=`, greater than `>`, less than `<`, greater-or-equal-to `>=`, less-than-or-equal-to `<=`, different than `<>` operators. Notice that a boolean false value is `0.0d`, while any other double value is considered as `true`; logical true is returned as `1.0d`.

### String Operators
Concatenation `%%`, concatenation with a blank in between `++` (e.g. `"hello"++"world"` => `"hello world"`), concatenation with a blank-plus-blank sequence `+++` (e.g. `"hello"+++"world"` => `"hello + world"`).

String comparison, equal `==` and different `!=` (notice that strings are trimmed via `.Trim()` method before being compared!

String *length* comparison, longer than `>>` and shorter than `<<` (notice that strings are trimmed via `.Trim()` method before being compared!

### Functions
`IF` evaluates the first argument and, if it represents a truth value, the second argument is evaluated and returned, the second otherwise.

`IFBRK` evaluates the first argument and, if it represents a truth value, the argument is evaluated and returned, otherwise an `ExpressionBreakException` is raised.

#### Double-based Functions
`NOT` returns the logical negation of the double argument.

`RANDOM` returns a random number between 0 (inclusive) and 1 (exclusive).	

Trigonometric functions `SIN`, `COS`, `TAN`, `ASIN`, `ACOS`, `ATAN`, argument is an angle measured in radians.
`RAD` converts degrees to radians, and `DEG` converts an radians to degrees.

`MAX`, `MIN`, `AVG` return the maximum, the minimum and the average of the provided double arguments.
`ABS` returns the absolute value.

`LN` and `LOG` return the natural (base `e`) logarithm.

`SQRT` returns the square root of the argument.

`ROUND`, returns the rounded (0.5 based) double value of the argument.

`FLOOR` returns the greatest integer value lower than or equal to the argument.

`CEIL` returns the lowest integer value greater than or equal to the argument.

#### Array-based Functions
`JOIN` returns an array that is the concatenation of the arguments; if an argument is a double, it's treated as a single-value array.

`SORT` returns an array that contains a concatenation of the arguments, sorted in ascending order.

#### String-based Functions
`CONCAT` returns the strings joined (just like the `%%` operator, but with multiple arguments).

`STRLEN` returns the length of the string (not trimmed).

`STRCMP` compares the first string argument with all the other arguments and returns true or false.
`STRICMP` same as `STRCMP`, but ignoring case (case insensitive).

`IN` returns true if the first argument as a string is equal to one of the other arguments.

### Constants
`e` the base of the natural logarithms, aka the Napier's constant.

`PI` the greek-pi.

`NaN` a placeholder for the double "Not a Number" constant.

`null` same as `NaN`.

`TRUE` is `1.0d`.

`FALSE` is `0.0d`.
