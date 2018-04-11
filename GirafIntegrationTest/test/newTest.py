from integrate import TestCase, test

class Test(TestCase):
   "Simple test case"

   @test()
   def makeGunnarGuardian(self, check):
      "Simple test"
      a = 1
      b = 2
      check.equal(a,b, "Fucked up")

   @test(depends=["makeGunnarGuardian"])
   def other_test(self, check):
      "Always failing test"
      check.fail("Always")
