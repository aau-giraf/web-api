from integrate import TestCase, test

class Test(TestCase):
        "Simple test case"

        @test()
        def simple_test(self, check):
                check.equal(1,2)

        @test(depends=["simple_test"])
        def other_test(self, check):
                "Always failing test"
                check.fail("Always fails")
