/**
 * Copyright 2016 Technische Universität Darmstadt
 * 
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 * 
 *    http://www.apache.org/licenses/LICENSE-2.0
 * 
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

/*
 * Invoke the following command to regenerate the parser/lexer from the grammar:
 * 
 * C#/Win: antlr.bat TypeNaming.g4 -package KaVE.Commons.Model.Names.CSharp.Parser
 * Java/OSX: antlr TypeNaming.g4 -package ..? (TODO: correct path)
 *
 * Please note that you also have to comment out the language option to create Java files.
 */

grammar TypeNaming;

options { 
	language=CSharp;
}

@lexer::header {
/**
 * Copyright 2016 Technische Universität Darmstadt
 * 
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 * 
 *    http://www.apache.org/licenses/LICENSE-2.0
 * 
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */
}
@parser::header {
/**
 * Copyright 2016 Technische Universität Darmstadt
 * 
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 * 
 *    http://www.apache.org/licenses/LICENSE-2.0
 * 
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */
}

typeEOL : type EOL;
methodEOL: method EOL;
namespaceEOL: namespace EOL;
assemblyEOL: assembly EOL;
parameterNameEOL: formalParam EOL;
memberNameEOL: memberName EOL;
lambdaNameEOL: lambdaName EOL;

type: UNKNOWN | typeParameter | regularType | delegateType | arrayType;
typeParameter : id (WS? '->' WS? notTypeParameter)?;
notTypeParameter: regularType | delegateType | arrayType | UNKNOWN | id;
regularType: ( resolvedType | nestedType ) ',' WS? assembly;
delegateType: 'd:' method;
arrayType: 'arr(' POSNUM '):' type;

nestedType: 'n:' nestedTypeName '+' typeName;
nestedTypeName: nestedType | resolvedType;


resolvedType: namespace? typeName;
namespace : (id '.')+;
typeName: enumTypeName | possiblyGenericTypeName;
possiblyGenericTypeName: ( interfaceTypeName | structTypeName | simpleTypeName ) genericTypePart?;

enumTypeName: 'e:' simpleTypeName;
interfaceTypeName: 'i:' simpleTypeName;
structTypeName: 's:' simpleTypeName;
simpleTypeName: id;


genericTypePart: '\'' POSNUM '[' genericParam (',' genericParam)* ']';
genericParam: '[' typeParameter ']';

assembly: regularAssembly | UNKNOWN;
regularAssembly:  assemblyName WS? (',' WS? assemblyVersion)?;
assemblyVersion: num '.' num '.' num '.' num;
assemblyName: (id '.')* id;	

memberName: UNKNOWN | simpleMemberName | propertyName;
simpleMemberName: staticModifier? WS? methodDefinition;
propertyName: (staticModifier | propertyModifier)? WS? methodDefinition methodSignature?;
propertyModifier: 'get' | 'set' | bothPropertyModifiers;
bothPropertyModifiers: ('get' WS? 'set') | ('set' WS? 'get');

lambdaName: UNKNOWN | realLambdaName;
realLambdaName: '[' type ']' WS? methodSignature;

method: UNKNOWN | regularMethod;
regularMethod: (nonStaticCtor | staticCctor | customMethod) methodSignature;
methodSignature: '(' WS? ( formalParam ( WS? ',' WS? formalParam)*)? WS? ')';
nonStaticCtor: WS? '[' UNKNOWN ']' WS? '[' type ']..ctor';
staticCctor: staticModifier WS? '[' UNKNOWN ']' WS? '[' type ']..cctor';
customMethod: staticModifier? WS? methodDefinition genericTypePart?;
methodDefinition: '[' type ']'  WS? '[' type '].' id;
formalParam: (WS? parameterModifier)? WS? '[' type ']' WS? id;
parameterModifier: paramsModifier | optsModifier | refModifier | outModifier | extensionModifier;

staticModifier: 'static';
paramsModifier: 'params ';
optsModifier: 'opt ';
refModifier: 'ref ';
outModifier: 'out ';
extensionModifier: 'this ';

// basic
UNKNOWN:'?';
id: LETTER (LETTER|num|SIGN)*;
num: '0' | POSNUM;
POSNUM:DIGIT_NON_ZERO DIGIT*;
LETTER:'a'..'z'|'A'..'Z';
SIGN:'+'|'-'|'*'|'/'|'_'|';'|':'|'='|'$'|'#'|'@'|'!';
fragment DIGIT:'0'|DIGIT_NON_ZERO;
fragment DIGIT_NON_ZERO: '1'..'9';
//WS: (' ' | '\t') -> skip;
WS: (' '| '\t')+;
EOL:'\n';
