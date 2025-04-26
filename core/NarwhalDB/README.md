# Ara3D.NarwhalDB

An extremely simple relational database-like data structure written in about 
250 lines of cross-platform (.NET Standard 2.0) C#. 

It provides a mechanism for representing related collections of objects 
as binary tables, and uses indices to store relationships. 

It offers the advantages of:

* Easy serialization in a [structured binary format (BFAST)](https://github.com/ara3d/ara3d/tree/main/src/Ara3D.Serialization.BFAST)
* BFAST can be read and written in different languages efficiently
* Reduced redundancy of data   
* Strings are deduplicated 

It does not support a query language, transactions, concurrency, 
or validation. Tables are limited to approximately 2billion rows

The primary use case is as an efficient representation for serialization 
or transmission of data, such as a native file format (e.g., 
https://www.sqlite.org/appfileformat.html). 

The secondary use case is as an adapater layer to arrange data so that 
it can subsequently be easily brought into a modern database system. 

## Why not SQLite? 

This library is designed for programmers who have very simple and specialized 
use cases of reading, writing, and transmitting relational data, but who don't
need the features of a modern relational database management system (RDMS) 
and don't want to bring in the additional complexity and dependencies. 

## Where does the name come from? 

NarwhalDB comes from "Not A Real DataBase", but NarwhalDB sounded better than 
"NardBase" (even though NardBase is funnier). Besides, Narwhals are awesome. 