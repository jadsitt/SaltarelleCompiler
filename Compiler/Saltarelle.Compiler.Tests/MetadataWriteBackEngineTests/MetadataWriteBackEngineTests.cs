﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using ICSharpCode.NRefactory.CSharp;
using ICSharpCode.NRefactory.TypeSystem;
using Mono.Cecil;
using NUnit.Framework;
using Saltarelle.Compiler.MetadataWriteBackEngine;
using Saltarelle.Compiler.Tests.MetadataWriteBackEngineTests.MetadataWriteBackEngineTestCase;

namespace Saltarelle.Compiler.Tests.MetadataWriteBackEngineTests {
	[TestFixture]
	public class MetadataWriteBackEngineTests {
    	private static readonly Lazy<IAssemblyReference> _currentAsmLazy = new Lazy<IAssemblyReference>(() => new CecilLoader() { IncludeInternalMembers = true }.LoadAssemblyFile(typeof(MetadataWriteBackEngineTests).Assembly.Location));
        internal static IAssemblyReference CurrentAsm { get { return _currentAsmLazy.Value; } }

		private void RunTest(Action<IMetadataWriteBackEngine, ICompilation> asserter) {
            IProjectContent project = new CSharpProjectContent();

            project = project.AddAssemblyReferences(new[] { Common.Mscorlib, CurrentAsm });

			var compilation = project.CreateCompilation();

			var asm = AssemblyDefinition.ReadAssembly(typeof(MetadataWriteBackEngineTests).Assembly.Location);
			var eng = new CecilMetadataWriteBackEngine(asm, compilation);

			asserter(eng, compilation);
		}

		[Test]
		public void CanGetAttributesOfType() {
			RunTest((engine, compilation) => {
				var attrs = engine.GetAttributes((ITypeDefinition)ReflectionHelper.ParseReflectionName("Saltarelle.Compiler.Tests.MetadataWriteBackEngineTests.MetadataWriteBackEngineTestCase.ClassWithAttribute").Resolve(compilation));
				Assert.That(attrs.Count, Is.EqualTo(1));
				var attr = attrs.ElementAt(0);
				Assert.That(attr.AttributeType, Is.EqualTo(ReflectionHelper.ParseReflectionName(typeof(MyAttribute).FullName).Resolve(compilation)));
				Assert.That(attr.PositionalArguments.Select(a => a.ConstantValue), Is.EqualTo(new[] { "This class has an attribute" }));
			});
		}

		[Test]
		public void CanGetAttributesOfField() {
			RunTest((engine, compilation) => {
				var fld = ReflectionHelper.ParseReflectionName(typeof(ClassWithAttributedField).FullName).Resolve(compilation).GetFields().Single(f => f.Name == "MyField");
				var attrs = engine.GetAttributes(fld);
				Assert.That(attrs.Count, Is.EqualTo(1));
				var attr = attrs.ElementAt(0);
				Assert.That(attr.AttributeType, Is.EqualTo(ReflectionHelper.ParseReflectionName(typeof(MyAttribute).FullName).Resolve(compilation)));
				Assert.That(attr.PositionalArguments.Select(a => a.ConstantValue), Is.EqualTo(new[] { "This field has an attribute" }));
			});
		}

		[Test]
		public void CanGetAttributesOfProperty() {
			RunTest((engine, compilation) => {
				var prop = ReflectionHelper.ParseReflectionName(typeof(ClassWithAttributedProperty).FullName).Resolve(compilation).GetProperties().Single(f => f.Name == "MyProperty");
				var attrs = engine.GetAttributes(prop);
				Assert.That(attrs.Count, Is.EqualTo(1));
				var attr = attrs.ElementAt(0);
				Assert.That(attr.AttributeType, Is.EqualTo(ReflectionHelper.ParseReflectionName(typeof(MyAttribute).FullName).Resolve(compilation)));
				Assert.That(attr.PositionalArguments.Select(a => a.ConstantValue), Is.EqualTo(new[] { "This property has an attribute" }));
			});
		}

		[Test]
		public void CanGetAttributesOfPropertyWhichIsAnExplicitInterfaceImplementation() {
			RunTest((engine, compilation) => {
				var prop = ReflectionHelper.ParseReflectionName(typeof(ClassWithAttributedExplicitlyImplementedProperty).FullName).Resolve(compilation).GetProperties().Single(f => f.Name == "MyProperty");
				var attrs = engine.GetAttributes(prop);
				Assert.That(attrs.Count, Is.EqualTo(1));
				var attr = attrs.ElementAt(0);
				Assert.That(attr.AttributeType, Is.EqualTo(ReflectionHelper.ParseReflectionName(typeof(MyAttribute).FullName).Resolve(compilation)));
				Assert.That(attr.PositionalArguments.Select(a => a.ConstantValue), Is.EqualTo(new[] { "This property has an attribute" }));
			});
		}

		[Test]
		public void CanGetAttributesOfPropertyGetter() {
			RunTest((engine, compilation) => {
				var prop = ReflectionHelper.ParseReflectionName(typeof(ClassWithAttributedProperty).FullName).Resolve(compilation).GetProperties().Single(f => f.Name == "MyProperty").Getter;
				var attrs = engine.GetAttributes(prop);
				Assert.That(attrs.Count, Is.EqualTo(1));
				var attr = attrs.ElementAt(0);
				Assert.That(attr.AttributeType, Is.EqualTo(ReflectionHelper.ParseReflectionName(typeof(MyAttribute).FullName).Resolve(compilation)));
				Assert.That(attr.PositionalArguments.Select(a => a.ConstantValue), Is.EqualTo(new[] { "This getter has an attribute" }));
			});
		}

		[Test]
		public void CanGetAttributesOfPropertyGetterWhichIsAnExplicitInterfaceImplementation() {
			RunTest((engine, compilation) => {
				var prop = ReflectionHelper.ParseReflectionName(typeof(ClassWithAttributedExplicitlyImplementedProperty).FullName).Resolve(compilation).GetProperties().Single(f => f.Name == "MyProperty").Getter;
				var attrs = engine.GetAttributes(prop);
				Assert.That(attrs.Count, Is.EqualTo(1));
				var attr = attrs.ElementAt(0);
				Assert.That(attr.AttributeType, Is.EqualTo(ReflectionHelper.ParseReflectionName(typeof(MyAttribute).FullName).Resolve(compilation)));
				Assert.That(attr.PositionalArguments.Select(a => a.ConstantValue), Is.EqualTo(new[] { "This getter has an attribute" }));
			});
		}

		[Test]
		public void CanGetAttributesOfPropertySetter() {
			RunTest((engine, compilation) => {
				var prop = ReflectionHelper.ParseReflectionName(typeof(ClassWithAttributedProperty).FullName).Resolve(compilation).GetProperties().Single(f => f.Name == "MyProperty").Setter;
				var attrs = engine.GetAttributes(prop);
				Assert.That(attrs.Count, Is.EqualTo(1));
				var attr = attrs.ElementAt(0);
				Assert.That(attr.AttributeType, Is.EqualTo(ReflectionHelper.ParseReflectionName(typeof(MyAttribute).FullName).Resolve(compilation)));
				Assert.That(attr.PositionalArguments.Select(a => a.ConstantValue), Is.EqualTo(new[] { "This setter has an attribute" }));
			});
		}

		[Test]
		public void CanGetAttributesOfPropertySetterWhichIsAnExplicitInterfaceImplementation() {
			RunTest((engine, compilation) => {
				var prop = ReflectionHelper.ParseReflectionName(typeof(ClassWithAttributedExplicitlyImplementedProperty).FullName).Resolve(compilation).GetProperties().Single(f => f.Name == "MyProperty").Setter;
				var attrs = engine.GetAttributes(prop);
				Assert.That(attrs.Count, Is.EqualTo(1));
				var attr = attrs.ElementAt(0);
				Assert.That(attr.AttributeType, Is.EqualTo(ReflectionHelper.ParseReflectionName(typeof(MyAttribute).FullName).Resolve(compilation)));
				Assert.That(attr.PositionalArguments.Select(a => a.ConstantValue), Is.EqualTo(new[] { "This setter has an attribute" }));
			});
		}

		[Test]
		public void CanGetAttributesOfIndexer() {
			RunTest((engine, compilation) => {
				var indexer1 = ReflectionHelper.ParseReflectionName(typeof(ClassWithAttributedIndexers).FullName).Resolve(compilation).GetProperties().Single(p => p.Name == "Item" && p.Parameters.Count == 1);
				var attrs = engine.GetAttributes(indexer1);
				Assert.That(attrs.Count, Is.EqualTo(1));
				var attr = attrs.ElementAt(0);
				Assert.That(attr.AttributeType, Is.EqualTo(ReflectionHelper.ParseReflectionName(typeof(MyAttribute).FullName).Resolve(compilation)));
				Assert.That(attr.PositionalArguments.Select(a => a.ConstantValue), Is.EqualTo(new[] { "Indexer(int)" }));

				var indexer2 = ReflectionHelper.ParseReflectionName(typeof(ClassWithAttributedIndexers).FullName).Resolve(compilation).GetProperties().Single(p => p.Name == "Item" && p.Parameters.Count == 2 && p.Parameters[1].Type.FullName == "System.Int32");
				attrs = engine.GetAttributes(indexer2);
				Assert.That(attrs.Count, Is.EqualTo(1));
				attr = attrs.ElementAt(0);
				Assert.That(attr.AttributeType, Is.EqualTo(ReflectionHelper.ParseReflectionName(typeof(MyAttribute).FullName).Resolve(compilation)));
				Assert.That(attr.PositionalArguments.Select(a => a.ConstantValue), Is.EqualTo(new[] { "Indexer(int,int)" }));

				var indexer3 = ReflectionHelper.ParseReflectionName(typeof(ClassWithAttributedIndexers).FullName).Resolve(compilation).GetProperties().Single(p => p.Name == "Item" && p.Parameters.Count == 2 && p.Parameters[1].Type.FullName == "System.String");
				attrs = engine.GetAttributes(indexer3);
				Assert.That(attrs.Count, Is.EqualTo(1));
				attr = attrs.ElementAt(0);
				Assert.That(attr.AttributeType, Is.EqualTo(ReflectionHelper.ParseReflectionName(typeof(MyAttribute).FullName).Resolve(compilation)));
				Assert.That(attr.PositionalArguments.Select(a => a.ConstantValue), Is.EqualTo(new[] { "Indexer(int,string)" }));
			});
		}

		[Test]
		public void CanGetAttributesOfIndexerWhichIsAnExplicitInterfaceImplementation() {
			RunTest((engine, compilation) => {
				var indexer1 = ReflectionHelper.ParseReflectionName(typeof(ClassWithAttributedExplicitlyImplementedIndexers).FullName).Resolve(compilation).GetProperties().Single(p => p.Name == "Item" && p.Parameters.Count == 1);
				var attrs = engine.GetAttributes(indexer1);
				Assert.That(attrs.Count, Is.EqualTo(1));
				var attr = attrs.ElementAt(0);
				Assert.That(attr.AttributeType, Is.EqualTo(ReflectionHelper.ParseReflectionName(typeof(MyAttribute).FullName).Resolve(compilation)));
				Assert.That(attr.PositionalArguments.Select(a => a.ConstantValue), Is.EqualTo(new[] { "Indexer(int)" }));

				var indexer2 = ReflectionHelper.ParseReflectionName(typeof(ClassWithAttributedExplicitlyImplementedIndexers).FullName).Resolve(compilation).GetProperties().Single(p => p.Name == "Item" && p.Parameters.Count == 2 && p.Parameters[1].Type.FullName == "System.Int32");
				attrs = engine.GetAttributes(indexer2);
				Assert.That(attrs.Count, Is.EqualTo(1));
				attr = attrs.ElementAt(0);
				Assert.That(attr.AttributeType, Is.EqualTo(ReflectionHelper.ParseReflectionName(typeof(MyAttribute).FullName).Resolve(compilation)));
				Assert.That(attr.PositionalArguments.Select(a => a.ConstantValue), Is.EqualTo(new[] { "Indexer(int,int)" }));

				var indexer3 = ReflectionHelper.ParseReflectionName(typeof(ClassWithAttributedExplicitlyImplementedIndexers).FullName).Resolve(compilation).GetProperties().Single(p => p.Name == "Item" && p.Parameters.Count == 2 && p.Parameters[1].Type.FullName == "System.String");
				attrs = engine.GetAttributes(indexer3);
				Assert.That(attrs.Count, Is.EqualTo(1));
				attr = attrs.ElementAt(0);
				Assert.That(attr.AttributeType, Is.EqualTo(ReflectionHelper.ParseReflectionName(typeof(MyAttribute).FullName).Resolve(compilation)));
				Assert.That(attr.PositionalArguments.Select(a => a.ConstantValue), Is.EqualTo(new[] { "Indexer(int,string)" }));
			});
		}

		[Test]
		public void CanGetAttributesOfIndexerGetter() {
			RunTest((engine, compilation) => {
				var indexer1 = ReflectionHelper.ParseReflectionName(typeof(ClassWithAttributedIndexers).FullName).Resolve(compilation).GetProperties().Single(p => p.Name == "Item" && p.Parameters.Count == 1).Getter;
				var attrs = engine.GetAttributes(indexer1);
				Assert.That(attrs.Count, Is.EqualTo(1));
				var attr = attrs.ElementAt(0);
				Assert.That(attr.AttributeType, Is.EqualTo(ReflectionHelper.ParseReflectionName(typeof(MyAttribute).FullName).Resolve(compilation)));
				Assert.That(attr.PositionalArguments.Select(a => a.ConstantValue), Is.EqualTo(new[] { "Indexer(int) getter" }));

				var indexer2 = ReflectionHelper.ParseReflectionName(typeof(ClassWithAttributedIndexers).FullName).Resolve(compilation).GetProperties().Single(p => p.Name == "Item" && p.Parameters.Count == 2 && p.Parameters[1].Type.FullName == "System.Int32").Getter;
				attrs = engine.GetAttributes(indexer2);
				Assert.That(attrs.Count, Is.EqualTo(1));
				attr = attrs.ElementAt(0);
				Assert.That(attr.AttributeType, Is.EqualTo(ReflectionHelper.ParseReflectionName(typeof(MyAttribute).FullName).Resolve(compilation)));
				Assert.That(attr.PositionalArguments.Select(a => a.ConstantValue), Is.EqualTo(new[] { "Indexer(int,int) getter" }));

				var indexer3 = ReflectionHelper.ParseReflectionName(typeof(ClassWithAttributedIndexers).FullName).Resolve(compilation).GetProperties().Single(p => p.Name == "Item" && p.Parameters.Count == 2 && p.Parameters[1].Type.FullName == "System.String").Getter;
				attrs = engine.GetAttributes(indexer3);
				Assert.That(attrs.Count, Is.EqualTo(1));
				attr = attrs.ElementAt(0);
				Assert.That(attr.AttributeType, Is.EqualTo(ReflectionHelper.ParseReflectionName(typeof(MyAttribute).FullName).Resolve(compilation)));
				Assert.That(attr.PositionalArguments.Select(a => a.ConstantValue), Is.EqualTo(new[] { "Indexer(int,string) getter" }));
			});
		}

		[Test]
		public void CanGetAttributesOfIndexerGetterWhichIsAnExplicitInterfaceImplementation() {
			RunTest((engine, compilation) => {
				var indexer1 = ReflectionHelper.ParseReflectionName(typeof(ClassWithAttributedExplicitlyImplementedIndexers).FullName).Resolve(compilation).GetProperties().Single(p => p.Name == "Item" && p.Parameters.Count == 1).Getter;
				var attrs = engine.GetAttributes(indexer1);
				Assert.That(attrs.Count, Is.EqualTo(1));
				var attr = attrs.ElementAt(0);
				Assert.That(attr.AttributeType, Is.EqualTo(ReflectionHelper.ParseReflectionName(typeof(MyAttribute).FullName).Resolve(compilation)));
				Assert.That(attr.PositionalArguments.Select(a => a.ConstantValue), Is.EqualTo(new[] { "Indexer(int) getter" }));

				var indexer2 = ReflectionHelper.ParseReflectionName(typeof(ClassWithAttributedExplicitlyImplementedIndexers).FullName).Resolve(compilation).GetProperties().Single(p => p.Name == "Item" && p.Parameters.Count == 2 && p.Parameters[1].Type.FullName == "System.Int32").Getter;
				attrs = engine.GetAttributes(indexer2);
				Assert.That(attrs.Count, Is.EqualTo(1));
				attr = attrs.ElementAt(0);
				Assert.That(attr.AttributeType, Is.EqualTo(ReflectionHelper.ParseReflectionName(typeof(MyAttribute).FullName).Resolve(compilation)));
				Assert.That(attr.PositionalArguments.Select(a => a.ConstantValue), Is.EqualTo(new[] { "Indexer(int,int) getter" }));

				var indexer3 = ReflectionHelper.ParseReflectionName(typeof(ClassWithAttributedExplicitlyImplementedIndexers).FullName).Resolve(compilation).GetProperties().Single(p => p.Name == "Item" && p.Parameters.Count == 2 && p.Parameters[1].Type.FullName == "System.String").Getter;
				attrs = engine.GetAttributes(indexer3);
				Assert.That(attrs.Count, Is.EqualTo(1));
				attr = attrs.ElementAt(0);
				Assert.That(attr.AttributeType, Is.EqualTo(ReflectionHelper.ParseReflectionName(typeof(MyAttribute).FullName).Resolve(compilation)));
				Assert.That(attr.PositionalArguments.Select(a => a.ConstantValue), Is.EqualTo(new[] { "Indexer(int,string) getter" }));
			});
		}

		[Test]
		public void CanGetAttributesOfIndexerSetter() {
			RunTest((engine, compilation) => {
				var indexer1 = ReflectionHelper.ParseReflectionName(typeof(ClassWithAttributedIndexers).FullName).Resolve(compilation).GetProperties().Single(p => p.Name == "Item" && p.Parameters.Count == 1).Setter;
				var attrs = engine.GetAttributes(indexer1);
				Assert.That(attrs.Count, Is.EqualTo(1));
				var attr = attrs.ElementAt(0);
				Assert.That(attr.AttributeType, Is.EqualTo(ReflectionHelper.ParseReflectionName(typeof(MyAttribute).FullName).Resolve(compilation)));
				Assert.That(attr.PositionalArguments.Select(a => a.ConstantValue), Is.EqualTo(new[] { "Indexer(int) setter" }));

				var indexer2 = ReflectionHelper.ParseReflectionName(typeof(ClassWithAttributedIndexers).FullName).Resolve(compilation).GetProperties().Single(p => p.Name == "Item" && p.Parameters.Count == 2 && p.Parameters[1].Type.FullName == "System.Int32").Setter;
				attrs = engine.GetAttributes(indexer2);
				Assert.That(attrs.Count, Is.EqualTo(1));
				attr = attrs.ElementAt(0);
				Assert.That(attr.AttributeType, Is.EqualTo(ReflectionHelper.ParseReflectionName(typeof(MyAttribute).FullName).Resolve(compilation)));
				Assert.That(attr.PositionalArguments.Select(a => a.ConstantValue), Is.EqualTo(new[] { "Indexer(int,int) setter" }));

				var indexer3 = ReflectionHelper.ParseReflectionName(typeof(ClassWithAttributedIndexers).FullName).Resolve(compilation).GetProperties().Single(p => p.Name == "Item" && p.Parameters.Count == 2 && p.Parameters[1].Type.FullName == "System.String").Setter;
				attrs = engine.GetAttributes(indexer3);
				Assert.That(attrs.Count, Is.EqualTo(1));
				attr = attrs.ElementAt(0);
				Assert.That(attr.AttributeType, Is.EqualTo(ReflectionHelper.ParseReflectionName(typeof(MyAttribute).FullName).Resolve(compilation)));
				Assert.That(attr.PositionalArguments.Select(a => a.ConstantValue), Is.EqualTo(new[] { "Indexer(int,string) setter" }));
			});
		}

		[Test]
		public void CanGetAttributesOfIndexerSetterWhichIsAnExplicitInterfaceImplementation() {
			RunTest((engine, compilation) => {
				var indexer1 = ReflectionHelper.ParseReflectionName(typeof(ClassWithAttributedExplicitlyImplementedIndexers).FullName).Resolve(compilation).GetProperties().Single(p => p.Name == "Item" && p.Parameters.Count == 1).Setter;
				var attrs = engine.GetAttributes(indexer1);
				Assert.That(attrs.Count, Is.EqualTo(1));
				var attr = attrs.ElementAt(0);
				Assert.That(attr.AttributeType, Is.EqualTo(ReflectionHelper.ParseReflectionName(typeof(MyAttribute).FullName).Resolve(compilation)));
				Assert.That(attr.PositionalArguments.Select(a => a.ConstantValue), Is.EqualTo(new[] { "Indexer(int) setter" }));

				var indexer2 = ReflectionHelper.ParseReflectionName(typeof(ClassWithAttributedExplicitlyImplementedIndexers).FullName).Resolve(compilation).GetProperties().Single(p => p.Name == "Item" && p.Parameters.Count == 2 && p.Parameters[1].Type.FullName == "System.Int32").Setter;
				attrs = engine.GetAttributes(indexer2);
				Assert.That(attrs.Count, Is.EqualTo(1));
				attr = attrs.ElementAt(0);
				Assert.That(attr.AttributeType, Is.EqualTo(ReflectionHelper.ParseReflectionName(typeof(MyAttribute).FullName).Resolve(compilation)));
				Assert.That(attr.PositionalArguments.Select(a => a.ConstantValue), Is.EqualTo(new[] { "Indexer(int,int) setter" }));

				var indexer3 = ReflectionHelper.ParseReflectionName(typeof(ClassWithAttributedExplicitlyImplementedIndexers).FullName).Resolve(compilation).GetProperties().Single(p => p.Name == "Item" && p.Parameters.Count == 2 && p.Parameters[1].Type.FullName == "System.String").Setter;
				attrs = engine.GetAttributes(indexer3);
				Assert.That(attrs.Count, Is.EqualTo(1));
				attr = attrs.ElementAt(0);
				Assert.That(attr.AttributeType, Is.EqualTo(ReflectionHelper.ParseReflectionName(typeof(MyAttribute).FullName).Resolve(compilation)));
				Assert.That(attr.PositionalArguments.Select(a => a.ConstantValue), Is.EqualTo(new[] { "Indexer(int,string) setter" }));
			});
		}

		[Test]
		public void CanGetAttributesOfEvent() {
			RunTest((engine, compilation) => {
				var prop = ReflectionHelper.ParseReflectionName(typeof(ClassWithAttributedEvent).FullName).Resolve(compilation).GetEvents().Single(e => e.Name == "MyEvent");
				var attrs = engine.GetAttributes(prop);
				Assert.That(attrs.Count, Is.EqualTo(1));
				var attr = attrs.ElementAt(0);
				Assert.That(attr.AttributeType, Is.EqualTo(ReflectionHelper.ParseReflectionName(typeof(MyAttribute).FullName).Resolve(compilation)));
				Assert.That(attr.PositionalArguments.Select(a => a.ConstantValue), Is.EqualTo(new[] { "This event has an attribute" }));
			});
		}

		[Test]
		public void CanGetAttributesOfEventAdder() {
			RunTest((engine, compilation) => {
				var prop = ReflectionHelper.ParseReflectionName(typeof(ClassWithAttributedEvent).FullName).Resolve(compilation).GetEvents().Single(e => e.Name == "MyEvent").AddAccessor;
				var attrs = engine.GetAttributes(prop);
				Assert.That(attrs.Count, Is.EqualTo(1));
				var attr = attrs.ElementAt(0);
				Assert.That(attr.AttributeType, Is.EqualTo(ReflectionHelper.ParseReflectionName(typeof(MyAttribute).FullName).Resolve(compilation)));
				Assert.That(attr.PositionalArguments.Select(a => a.ConstantValue), Is.EqualTo(new[] { "This event adder has an attribute" }));
			});
		}

		[Test]
		public void CanGetAttributesOfEventRemover() {
			RunTest((engine, compilation) => {
				var prop = ReflectionHelper.ParseReflectionName(typeof(ClassWithAttributedEvent).FullName).Resolve(compilation).GetEvents().Single(e => e.Name == "MyEvent").RemoveAccessor;
				var attrs = engine.GetAttributes(prop);
				Assert.That(attrs.Count, Is.EqualTo(1));
				var attr = attrs.ElementAt(0);
				Assert.That(attr.AttributeType, Is.EqualTo(ReflectionHelper.ParseReflectionName(typeof(MyAttribute).FullName).Resolve(compilation)));
				Assert.That(attr.PositionalArguments.Select(a => a.ConstantValue), Is.EqualTo(new[] { "This event remover has an attribute" }));
			});
		}

		[Test]
		public void CanGetAttributesOfEventWhichIsAnExplicitInterfaceImplementation() {
			RunTest((engine, compilation) => {
				var prop = ReflectionHelper.ParseReflectionName(typeof(ClassWithAttributedExplicitlyImplementedEventAccessors).FullName).Resolve(compilation).GetEvents().Single(e => e.Name == "MyEvent");
				var attrs = engine.GetAttributes(prop);
				Assert.That(attrs.Count, Is.EqualTo(1));
				var attr = attrs.ElementAt(0);
				Assert.That(attr.AttributeType, Is.EqualTo(ReflectionHelper.ParseReflectionName(typeof(MyAttribute).FullName).Resolve(compilation)));
				Assert.That(attr.PositionalArguments.Select(a => a.ConstantValue), Is.EqualTo(new[] { "This event has an attribute" }));
			});
		}

		[Test]
		public void CanGetAttributesOfEventAdderWhichIsAnExplicitInterfaceImplementation() {
			RunTest((engine, compilation) => {
				var prop = ReflectionHelper.ParseReflectionName(typeof(ClassWithAttributedExplicitlyImplementedEventAccessors).FullName).Resolve(compilation).GetEvents().Single(e => e.Name == "MyEvent").AddAccessor;
				var attrs = engine.GetAttributes(prop);
				Assert.That(attrs.Count, Is.EqualTo(1));
				var attr = attrs.ElementAt(0);
				Assert.That(attr.AttributeType, Is.EqualTo(ReflectionHelper.ParseReflectionName(typeof(MyAttribute).FullName).Resolve(compilation)));
				Assert.That(attr.PositionalArguments.Select(a => a.ConstantValue), Is.EqualTo(new[] { "This event adder has an attribute" }));
			});
		}

		[Test]
		public void CanGetAttributesOfEventRemoverWhichIsAnExplicitInterfaceImplementation() {
			RunTest((engine, compilation) => {
				var prop = ReflectionHelper.ParseReflectionName(typeof(ClassWithAttributedExplicitlyImplementedEventAccessors).FullName).Resolve(compilation).GetEvents().Single(e => e.Name == "MyEvent").RemoveAccessor;
				var attrs = engine.GetAttributes(prop);
				Assert.That(attrs.Count, Is.EqualTo(1));
				var attr = attrs.ElementAt(0);
				Assert.That(attr.AttributeType, Is.EqualTo(ReflectionHelper.ParseReflectionName(typeof(MyAttribute).FullName).Resolve(compilation)));
				Assert.That(attr.PositionalArguments.Select(a => a.ConstantValue), Is.EqualTo(new[] { "This event remover has an attribute" }));
			});
		}

		[Test]
		public void CanGetAttributesOfMethod() {
			RunTest((engine, compilation) => {
				var method1 = ReflectionHelper.ParseReflectionName(typeof(ClassWithMethods).FullName).Resolve(compilation).GetMethods().Single(p => p.Name == "MyMethod" && p.Parameters.Count == 0);
				var attrs = engine.GetAttributes(method1);
				Assert.That(attrs.Count, Is.EqualTo(1));
				var attr = attrs.ElementAt(0);
				Assert.That(attr.AttributeType, Is.EqualTo(ReflectionHelper.ParseReflectionName(typeof(MyAttribute).FullName).Resolve(compilation)));
				Assert.That(attr.PositionalArguments.Select(a => a.ConstantValue), Is.EqualTo(new[] { "MyMethod()" }));

				var method2 = ReflectionHelper.ParseReflectionName(typeof(ClassWithMethods).FullName).Resolve(compilation).GetMethods().Single(p => p.Name == "MyMethod" && p.Parameters.Count == 1);
				attrs = engine.GetAttributes(method2);
				Assert.That(attrs.Count, Is.EqualTo(1));
				attr = attrs.ElementAt(0);
				Assert.That(attr.AttributeType, Is.EqualTo(ReflectionHelper.ParseReflectionName(typeof(MyAttribute).FullName).Resolve(compilation)));
				Assert.That(attr.PositionalArguments.Select(a => a.ConstantValue), Is.EqualTo(new[] { "MyMethod(int)" }));

				var method3 = ReflectionHelper.ParseReflectionName(typeof(ClassWithMethods).FullName).Resolve(compilation).GetMethods().Single(p => p.Name == "MyMethod" && p.Parameters.Count == 2 && p.Parameters[1].Type.FullName == "System.Int32");
				attrs = engine.GetAttributes(method3);
				Assert.That(attrs.Count, Is.EqualTo(1));
				attr = attrs.ElementAt(0);
				Assert.That(attr.AttributeType, Is.EqualTo(ReflectionHelper.ParseReflectionName(typeof(MyAttribute).FullName).Resolve(compilation)));
				Assert.That(attr.PositionalArguments.Select(a => a.ConstantValue), Is.EqualTo(new[] { "MyMethod(int,int)" }));

				var method4 = ReflectionHelper.ParseReflectionName(typeof(ClassWithMethods).FullName).Resolve(compilation).GetMethods().Single(p => p.Name == "MyMethod" && p.Parameters.Count == 2 && p.Parameters[1].Type.FullName == "System.String");
				attrs = engine.GetAttributes(method4);
				Assert.That(attrs.Count, Is.EqualTo(1));
				attr = attrs.ElementAt(0);
				Assert.That(attr.AttributeType, Is.EqualTo(ReflectionHelper.ParseReflectionName(typeof(MyAttribute).FullName).Resolve(compilation)));
				Assert.That(attr.PositionalArguments.Select(a => a.ConstantValue), Is.EqualTo(new[] { "MyMethod(int,string)" }));
			});
		}

		[Test]
		public void CanGetAttributesOfMethodWhichIsAnExplicitInterfaceImplementation() {
			RunTest((engine, compilation) => {
				var method1 = ReflectionHelper.ParseReflectionName(typeof(ClassWithMethods).FullName).Resolve(compilation).GetMethods().Single(p => p.Name == "MyMethod" && p.Parameters.Count == 0);
				var attrs = engine.GetAttributes(method1);
				Assert.That(attrs.Count, Is.EqualTo(1));
				var attr = attrs.ElementAt(0);
				Assert.That(attr.AttributeType, Is.EqualTo(ReflectionHelper.ParseReflectionName(typeof(MyAttribute).FullName).Resolve(compilation)));
				Assert.That(attr.PositionalArguments.Select(a => a.ConstantValue), Is.EqualTo(new[] { "MyMethod()" }));

				var method2 = ReflectionHelper.ParseReflectionName(typeof(ClassWithMethods).FullName).Resolve(compilation).GetMethods().Single(p => p.Name == "MyMethod" && p.Parameters.Count == 1);
				attrs = engine.GetAttributes(method2);
				Assert.That(attrs.Count, Is.EqualTo(1));
				attr = attrs.ElementAt(0);
				Assert.That(attr.AttributeType, Is.EqualTo(ReflectionHelper.ParseReflectionName(typeof(MyAttribute).FullName).Resolve(compilation)));
				Assert.That(attr.PositionalArguments.Select(a => a.ConstantValue), Is.EqualTo(new[] { "MyMethod(int)" }));

				var method3 = ReflectionHelper.ParseReflectionName(typeof(ClassWithMethods).FullName).Resolve(compilation).GetMethods().Single(p => p.Name == "MyMethod" && p.Parameters.Count == 2 && p.Parameters[1].Type.FullName == "System.Int32");
				attrs = engine.GetAttributes(method3);
				Assert.That(attrs.Count, Is.EqualTo(1));
				attr = attrs.ElementAt(0);
				Assert.That(attr.AttributeType, Is.EqualTo(ReflectionHelper.ParseReflectionName(typeof(MyAttribute).FullName).Resolve(compilation)));
				Assert.That(attr.PositionalArguments.Select(a => a.ConstantValue), Is.EqualTo(new[] { "MyMethod(int,int)" }));

				var method4 = ReflectionHelper.ParseReflectionName(typeof(ClassWithMethods).FullName).Resolve(compilation).GetMethods().Single(p => p.Name == "MyMethod" && p.Parameters.Count == 2 && p.Parameters[1].Type.FullName == "System.String");
				attrs = engine.GetAttributes(method4);
				Assert.That(attrs.Count, Is.EqualTo(1));
				attr = attrs.ElementAt(0);
				Assert.That(attr.AttributeType, Is.EqualTo(ReflectionHelper.ParseReflectionName(typeof(MyAttribute).FullName).Resolve(compilation)));
				Assert.That(attr.PositionalArguments.Select(a => a.ConstantValue), Is.EqualTo(new[] { "MyMethod(int,string)" }));
			});
		}

		[Test]
		public void CanGetAttributesOfMethodWhichIsAnExplicitInterfaceImplementationInAGenericClass() {
			RunTest((engine, compilation) => {
				var method1 = ReflectionHelper.ParseReflectionName(typeof(GenericClassWithAttributedExplicitlyImplementedMethods<>).FullName).Resolve(compilation).GetMethods().Single(p => p.Name == "MyMethod" && p.Parameters.Count == 0);
				var attrs = engine.GetAttributes(method1);
				Assert.That(attrs.Count, Is.EqualTo(1));
				var attr = attrs.ElementAt(0);
				Assert.That(attr.AttributeType, Is.EqualTo(ReflectionHelper.ParseReflectionName(typeof(MyAttribute).FullName).Resolve(compilation)));
				Assert.That(attr.PositionalArguments.Select(a => a.ConstantValue), Is.EqualTo(new[] { "MyMethod()" }));

				var method2 = ReflectionHelper.ParseReflectionName(typeof(GenericClassWithAttributedExplicitlyImplementedMethods<>).FullName).Resolve(compilation).GetMethods().Single(p => p.Name == "MyMethod" && p.Parameters.Count == 1);
				attrs = engine.GetAttributes(method2);
				Assert.That(attrs.Count, Is.EqualTo(1));
				attr = attrs.ElementAt(0);
				Assert.That(attr.AttributeType, Is.EqualTo(ReflectionHelper.ParseReflectionName(typeof(MyAttribute).FullName).Resolve(compilation)));
				Assert.That(attr.PositionalArguments.Select(a => a.ConstantValue), Is.EqualTo(new[] { "MyMethod(int)" }));

				var method3 = ReflectionHelper.ParseReflectionName(typeof(GenericClassWithAttributedExplicitlyImplementedMethods<>).FullName).Resolve(compilation).GetMethods().Single(p => p.Name == "MyMethod" && p.Parameters.Count == 2 && p.Parameters[1].Type.FullName == "System.Int32");
				attrs = engine.GetAttributes(method3);
				Assert.That(attrs.Count, Is.EqualTo(1));
				attr = attrs.ElementAt(0);
				Assert.That(attr.AttributeType, Is.EqualTo(ReflectionHelper.ParseReflectionName(typeof(MyAttribute).FullName).Resolve(compilation)));
				Assert.That(attr.PositionalArguments.Select(a => a.ConstantValue), Is.EqualTo(new[] { "MyMethod(int,int)" }));

				var method4 = ReflectionHelper.ParseReflectionName(typeof(GenericClassWithAttributedExplicitlyImplementedMethods<>).FullName).Resolve(compilation).GetMethods().Single(p => p.Name == "MyMethod" && p.Parameters.Count == 2 && p.Parameters[1].Type.FullName == "System.String");
				attrs = engine.GetAttributes(method4);
				Assert.That(attrs.Count, Is.EqualTo(1));
				attr = attrs.ElementAt(0);
				Assert.That(attr.AttributeType, Is.EqualTo(ReflectionHelper.ParseReflectionName(typeof(MyAttribute).FullName).Resolve(compilation)));
				Assert.That(attr.PositionalArguments.Select(a => a.ConstantValue), Is.EqualTo(new[] { "MyMethod(int,string)" }));
			});
		}

		[Test]
		public void CanGetAttributesOfOperator() {
			RunTest((engine, compilation) => {
				var operator1 = ReflectionHelper.ParseReflectionName(typeof(ClassWithOperators).FullName).Resolve(compilation).GetMethods().Single(p => p.Name == "op_Addition" && p.Parameters[1].Type.FullName != "System.Int32");
				var attrs = engine.GetAttributes(operator1);
				Assert.That(attrs.Count, Is.EqualTo(1));
				var attr = attrs.ElementAt(0);
				Assert.That(attr.AttributeType, Is.EqualTo(ReflectionHelper.ParseReflectionName(typeof(MyAttribute).FullName).Resolve(compilation)));
				Assert.That(attr.PositionalArguments.Select(a => a.ConstantValue), Is.EqualTo(new[] { "Add class instances" }));

				var operator2 = ReflectionHelper.ParseReflectionName(typeof(ClassWithOperators).FullName).Resolve(compilation).GetMethods().Single(p => p.Name == "op_Addition" && p.Parameters[1].Type.FullName == "System.Int32");
				attrs = engine.GetAttributes(operator2);
				Assert.That(attrs.Count, Is.EqualTo(1));
				attr = attrs.ElementAt(0);
				Assert.That(attr.AttributeType, Is.EqualTo(ReflectionHelper.ParseReflectionName(typeof(MyAttribute).FullName).Resolve(compilation)));
				Assert.That(attr.PositionalArguments.Select(a => a.ConstantValue), Is.EqualTo(new[] { "Add class and int" }));

				var operator3 = ReflectionHelper.ParseReflectionName(typeof(ClassWithOperators).FullName).Resolve(compilation).GetMethods().Single(p => p.Name == "op_Implicit" && p.ReturnType.FullName == "System.Int32");
				attrs = engine.GetAttributes(operator3);
				Assert.That(attrs.Count, Is.EqualTo(1));
				attr = attrs.ElementAt(0);
				Assert.That(attr.AttributeType, Is.EqualTo(ReflectionHelper.ParseReflectionName(typeof(MyAttribute).FullName).Resolve(compilation)));
				Assert.That(attr.PositionalArguments.Select(a => a.ConstantValue), Is.EqualTo(new[] { "Convert to int" }));

				var operator4 = ReflectionHelper.ParseReflectionName(typeof(ClassWithOperators).FullName).Resolve(compilation).GetMethods().Single(p => p.Name == "op_Implicit" && p.ReturnType.FullName == "System.Single");
				attrs = engine.GetAttributes(operator4);
				Assert.That(attrs.Count, Is.EqualTo(1));
				attr = attrs.ElementAt(0);
				Assert.That(attr.AttributeType, Is.EqualTo(ReflectionHelper.ParseReflectionName(typeof(MyAttribute).FullName).Resolve(compilation)));
				Assert.That(attr.PositionalArguments.Select(a => a.ConstantValue), Is.EqualTo(new[] { "Convert to float" }));

				var operator5 = ReflectionHelper.ParseReflectionName(typeof(ClassWithOperators).FullName).Resolve(compilation).GetMethods().Single(p => p.Name == "op_Explicit" && p.ReturnType.FullName == "System.String");
				attrs = engine.GetAttributes(operator5);
				Assert.That(attrs.Count, Is.EqualTo(1));
				attr = attrs.ElementAt(0);
				Assert.That(attr.AttributeType, Is.EqualTo(ReflectionHelper.ParseReflectionName(typeof(MyAttribute).FullName).Resolve(compilation)));
				Assert.That(attr.PositionalArguments.Select(a => a.ConstantValue), Is.EqualTo(new[] { "Convert to string" }));
			});
		}

		[Test]
		public void CanGetAttributesOfConstructor() {
			RunTest((engine, compilation) => {
				var ctor1 = ReflectionHelper.ParseReflectionName(typeof(ClassWithConstructors).FullName).Resolve(compilation).GetConstructors().Single(c => c.Parameters.Count == 0);
				var attrs = engine.GetAttributes(ctor1);
				Assert.That(attrs.Count, Is.EqualTo(1));
				var attr = attrs.ElementAt(0);
				Assert.That(attr.AttributeType, Is.EqualTo(ReflectionHelper.ParseReflectionName(typeof(MyAttribute).FullName).Resolve(compilation)));
				Assert.That(attr.PositionalArguments.Select(a => a.ConstantValue), Is.EqualTo(new[] { "Constructor()" }));

				var ctor2 = ReflectionHelper.ParseReflectionName(typeof(ClassWithConstructors).FullName).Resolve(compilation).GetConstructors().Single(c => c.Parameters.Count == 1);
				attrs = engine.GetAttributes(ctor2);
				Assert.That(attrs.Count, Is.EqualTo(1));
				attr = attrs.ElementAt(0);
				Assert.That(attr.AttributeType, Is.EqualTo(ReflectionHelper.ParseReflectionName(typeof(MyAttribute).FullName).Resolve(compilation)));
				Assert.That(attr.PositionalArguments.Select(a => a.ConstantValue), Is.EqualTo(new[] { "Constructor(int)" }));

				var ctor3 = ReflectionHelper.ParseReflectionName(typeof(ClassWithConstructors).FullName).Resolve(compilation).GetConstructors().Single(c => c.Parameters.Count == 2 && c.Parameters[1].Type.FullName == "System.Int32");
				attrs = engine.GetAttributes(ctor3);
				Assert.That(attrs.Count, Is.EqualTo(1));
				attr = attrs.ElementAt(0);
				Assert.That(attr.AttributeType, Is.EqualTo(ReflectionHelper.ParseReflectionName(typeof(MyAttribute).FullName).Resolve(compilation)));
				Assert.That(attr.PositionalArguments.Select(a => a.ConstantValue), Is.EqualTo(new[] { "Constructor(int,int)" }));

				var ctor4 = ReflectionHelper.ParseReflectionName(typeof(ClassWithConstructors).FullName).Resolve(compilation).GetConstructors().Single(c => c.Parameters.Count == 2 && c.Parameters[1].Type.FullName == "System.String");
				attrs = engine.GetAttributes(ctor4);
				Assert.That(attrs.Count, Is.EqualTo(1));
				attr = attrs.ElementAt(0);
				Assert.That(attr.AttributeType, Is.EqualTo(ReflectionHelper.ParseReflectionName(typeof(MyAttribute).FullName).Resolve(compilation)));
				Assert.That(attr.PositionalArguments.Select(a => a.ConstantValue), Is.EqualTo(new[] { "Constructor(int,string)" }));
			});
		}

		[Test]
		public void PositionalArgumentsWork() {
			Assert.Fail("TODO");
		}

		[Test]
		public void NamedArgumentsWork() {
			Assert.Fail("TODO");
		}

		[Test]
		public void ConstructorCanBeResolved() {
			Assert.Fail("TODO");
		}
	}
}