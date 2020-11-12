using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using QUT.Gppg;

namespace SimpleParser
{
public enum Tokens {
    error=1,EOF=2,SCRIPT=3,MOTION=4,TURN=5,TAKE=6,
    PUT=7,SEAL=8,FORWARD=9,BACK=10,LEFT=11,RIGHT=12,
    ON=13,BRACKETL=14,BRACKETR=15,BRACEL=16,BRACER=17,SEMICOLON=18,
    NUMBER=19,TEXT=20};

public abstract class ScanBase : AbstractScanner<int,LexLocation> {
  private LexLocation __yylloc = new LexLocation();
  public override LexLocation yylloc { get { return __yylloc; } set { __yylloc = value; } }
  protected virtual bool yywrap() { return true; }
}

public class Parser: ShiftReduceParser<int, LexLocation>
{
    public Parser(AbstractScanner<int, LexLocation> scanner) : base(scanner) { }

  private static Dictionary<int, string> aliasses;
  private static Rule[] rules = new Rule[20];
  private static State[] states = new State[40];
  private static string[] nonTerms = new string[] {
      "program", "$accept", "block", "stlist", "statement", "mov", "turn", "take", 
      "put", "seal", "way", "side", };

  static Parser()
  {
    states[0] = new State(new int[]{3,3},new int[]{-1,1});
    states[1] = new State(new int[]{2,2});
    states[2] = new State(-1);
    states[3] = new State(new int[]{20,4});
    states[4] = new State(new int[]{16,6},new int[]{-3,5});
    states[5] = new State(-2);
    states[6] = new State(new int[]{4,11,5,21,6,31,7,34,8,37},new int[]{-4,7,-5,39,-6,10,-7,20,-8,30,-9,33,-10,36});
    states[7] = new State(new int[]{17,8,4,11,5,21,6,31,7,34,8,37},new int[]{-5,9,-6,10,-7,20,-8,30,-9,33,-10,36});
    states[8] = new State(-3);
    states[9] = new State(-5);
    states[10] = new State(-6);
    states[11] = new State(new int[]{9,18,10,19},new int[]{-11,12});
    states[12] = new State(new int[]{13,13});
    states[13] = new State(new int[]{14,14});
    states[14] = new State(new int[]{19,15});
    states[15] = new State(new int[]{15,16});
    states[16] = new State(new int[]{18,17});
    states[17] = new State(-11);
    states[18] = new State(-12);
    states[19] = new State(-13);
    states[20] = new State(-7);
    states[21] = new State(new int[]{11,28,12,29},new int[]{-12,22});
    states[22] = new State(new int[]{13,23});
    states[23] = new State(new int[]{14,24});
    states[24] = new State(new int[]{19,25});
    states[25] = new State(new int[]{15,26});
    states[26] = new State(new int[]{18,27});
    states[27] = new State(-14);
    states[28] = new State(-15);
    states[29] = new State(-16);
    states[30] = new State(-8);
    states[31] = new State(new int[]{18,32});
    states[32] = new State(-17);
    states[33] = new State(-9);
    states[34] = new State(new int[]{18,35});
    states[35] = new State(-18);
    states[36] = new State(-10);
    states[37] = new State(new int[]{18,38});
    states[38] = new State(-19);
    states[39] = new State(-4);

    rules[1] = new Rule(-2, new int[]{-1,2});
    rules[2] = new Rule(-1, new int[]{3,20,-3});
    rules[3] = new Rule(-3, new int[]{16,-4,17});
    rules[4] = new Rule(-4, new int[]{-5});
    rules[5] = new Rule(-4, new int[]{-4,-5});
    rules[6] = new Rule(-5, new int[]{-6});
    rules[7] = new Rule(-5, new int[]{-7});
    rules[8] = new Rule(-5, new int[]{-8});
    rules[9] = new Rule(-5, new int[]{-9});
    rules[10] = new Rule(-5, new int[]{-10});
    rules[11] = new Rule(-6, new int[]{4,-11,13,14,19,15,18});
    rules[12] = new Rule(-11, new int[]{9});
    rules[13] = new Rule(-11, new int[]{10});
    rules[14] = new Rule(-7, new int[]{5,-12,13,14,19,15,18});
    rules[15] = new Rule(-12, new int[]{11});
    rules[16] = new Rule(-12, new int[]{12});
    rules[17] = new Rule(-8, new int[]{6,18});
    rules[18] = new Rule(-9, new int[]{7,18});
    rules[19] = new Rule(-10, new int[]{8,18});
  }

  protected override void Initialize()
        {
    this.InitSpecialTokens((int)Tokens.error, (int)Tokens.EOF);
    this.InitStates(states);
    this.InitRules(rules);
    this.InitNonTerminals(nonTerms);
  }

  protected override void DoAction(int action)
  { }

  protected override string TerminalToString(int terminal)
  {
    if (aliasses != null && aliasses.ContainsKey(terminal))
        return aliasses[terminal];
    else if (((Tokens)terminal).ToString() != terminal.ToString(CultureInfo.InvariantCulture))
        return ((Tokens)terminal).ToString();
    else
        return CharToString((char)terminal);
  }

}
}
