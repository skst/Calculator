﻿# CalculateX [![.NET](https://github.com/skst/CalculateX/actions/workflows/dotnet.yml/badge.svg)](https://github.com/skst/CalculateX/actions/workflows/dotnet.yml)

A calculator that evaluates math expressions.

[12noon.com](https://12noon.com)

<a href='//www.microsoft.com/store/apps/9NWLKMVZPJD3?cid=storebadge&ocid=badge'>
    <img src='https://developer.microsoft.com/store/badges/images/English_get-it-from-MS.png' alt='English badge' width="284" height="104" />
</a>

![CalculateX](https://github.com/skst/CalculateX/blob/master/CalculateX.png)

## Features

### Summary
- Evaluates math expressions including grouping.
- Supports trigonometry and other functions.
- Converts common units of:
    - Length
    - Speed
    - Temperature
    - Time
    - Mass
    - Volume
- Supports variables including last answer.

Enter simple or complex mathematical expressions and immediately calculate the result.
From adding your grocery bill to calculating cosine, *CalculateX* does it all.
Results are automatically added to a history record, so you don't have to write them down
to remember them.
Those results are even saved and restored when you exit and restart the application.

### Usage

Type an expression in the input field and press **Enter**.

Press **ESC** to clear the input field.

Press the **up-** and **down-arrows** to cycle through previous inputs.

If you use the mouse to select text in the history, it is automatically copied to the clipboard.
The focus is also set on the input field so that you can quickly paste the selected text if desired.
When pasting a number, any currency symbol and grouping separators are removed
since they are not allowed in expressions.

### Basic Mathematical Operators

*CalculateX* supports the basic mathematical operators: addition, subtraction,
multiplication, division, modulo, and exponent.
You can use parentheses to group expressions and give them precedence.

- addition: `+`
- subtraction: `-`
- multiplication: `*`
- division: `/`
- modulo: `%`
- exponent: `^`

Example: `(3 + 4)*(5 - 2) * (sin(68) / sqrt(25)) + max(16, 7) ^ 2`

You can also use algebraic (implied) multiplication in many cases:

- Between groups: `(3 + 4)(5 - 2)`
- Before and after groups: `-17(9 / 3)42`
- Before and after functions: `23sqrt(9)ceiling(25 / 2)12`
- Between groups and functions: `(4 + 3)sqrt(9)(12 / 2)`
- Between groups and variables: `(4 + 3)e(12 / 2)`
- Before variables: `4pi`

### Variables

You can create variables and access them in your expressions.
Variables can contain letters, digits, and underscores, but they must start with a letter.
All variables and their current values are displayed in the list on the right.

Examples:
````
x = 23.68
y2 = sqrt(146)
x2y = x * y2
result = 12 + 8.4
subtotal3 = result - x2y + 87
````

The `pi` and `e` variables are pre-defined.
The `answer` variable always equals the most recent result.

Examples:
````
y = sin(pi / 2)
y2 = asin(2pi)
x = y + 2
x + 1
````
(The result is `4` and the `answer` variable is automatically set to `4`.)

📌 If you type an operator into an empty input field,
it automatically enters "*answer*" in the field for you.

You can clear a variable (and remove it from the variable pane) by setting it to nothing.

````
x =
````

### Basic Functions

- sqrt(n) - square root of `n`
- cbrt(n) - cube root of `n`
- abs(n) - absolute value of `n`
- pow(n, x) - `n` to the power of `x`
- min(a, b) - smallest of `a` and `b`
- max(a, b) - largest of `a` and `b`
- round(d, n) - `d` rounded to `n` decimal places
- truncate(d) - `d` without decimal portion
- floor(n) - largest integer smaller than `n`
- ceiling(n) - smallest integer larger than `n`
- random1() - random number `r` less than 1 (0 <= r < 1)
- randomN(n) - random number `r` between 0 and `n` (0 <= r < n)

### Trigonometric Functions

- cos(t) - cosine of `t`
- cosh(t) - hyperbolic cosine of `t`
- acos(t) - arccosine of `t`
- sin(t) - sine of `t`
- sinh(t) - hyperbolic sine of `t`
- asin(t) - arcsine of `t`
- tan(t) - tangent of `t`
- tanh(t) - hyperbolic tangent of `t`
- atan(t) - arctangent of `t`
- atanh(t) - hyperbolic arctangent of `t`
- atan2(x,y) - arctangent of `x/y`

### Logarithmic Functions

- exp(x) - `e` to the power of `x`
- log(n) - log base `e` of `n`
- log10(n) - log base 10 of `n`
- log2(n) - log base 2 of `n`

### Conversion Functions

To convert between two units, use this format:

`n[from->to]`

Example: `80[kph->mph]`

#### length

- Kilometer `km`
- Meter `m`
- Centimeter `cm`
- Millimeter `mm`
- Mile `mile`
- Yard `yd`
- Feet `ft`
- Inch `in`

#### speed

- Kilometers/hour `kph`
- Meters/second `m/s`
- Miles/hour `mph`
- Feet/second `ft/s`
- Knot `knot` (nautical-miles/hour)
- Mach `mach` (speed of sound)

#### temperature

- Fahrenheit `f`
- Celsius `c`
- Kelvin `k`

#### time

- Week `wk`
- Day `d`
- Hour `hr`
- Minute `min`
- Second `sec`
- Millisecond `ms`

#### mass

- Kilogram `kg`
- Gram `g`
- Milligram `mg`
- Ton `ton`
- Pound `lg`
- Ounce `oz`

#### volume

- Kiloliter `kl`
- Liter `l`
- Milliliter `ml`
- Gallon `gal`
- Quart `qt`
- Pint `pt`
- Cup `cup`
- Ounce `oz`

------------

## MathExpressions Library

The library supports mathematical expressions, functions, unit conversion, and variables.

Below are some C# examples of using the library directly.

````C#
MathEvaluator eval = new();
//basic math
double result = eval.Evaluate("(2 + 1) * (1 + 2)");
//calling a function
result = eval.Evaluate("sqrt(4)");
//evaluate trigonometric 
result = eval.Evaluate("cos(pi * 45 / 180.0)");
//convert inches to feet
result = eval.Evaluate("12 [in->ft]");
//use variable
result = eval.Evaluate("answer * 10");
//add variable
eval.Variables.Add("x", 10);
result = eval.Evaluate("x * 10");
````

------------

*This project was originally forked from [Calculator.NET](https://github.com/loresoft/Calculator).*
