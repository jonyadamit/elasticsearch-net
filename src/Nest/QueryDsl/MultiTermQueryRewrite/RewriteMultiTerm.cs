﻿using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Runtime.Serialization;

namespace Nest
{
	[JsonConverter(typeof(StringEnumConverter))]
	public enum RewriteMultiTerm
	{
		/// <summary>
		///  A rewrite method that performs like constant_score_boolean when there are few matching terms and otherwise
		///  visits all matching terms in sequence and marks documents for that term. Matching documents are assigned a
		///  constant score equal to the query’s boost.
		/// </summary>
		[EnumMember(Value = "constant_score")]
		ConstantScore,
		/// <summary>
		/// A rewrite method that first translates each term into a should clause in a boolean query, and keeps the scores
		///  as computed by the query. Note that typically such scores are meaningless to the user, and require non-trivial
		///  CPU to compute, so it’s almost always better to use constant_score_auto. This rewrite method will hit too many
		///  clauses failure if it exceeds the boolean query limit (defaults to 1024).
		/// </summary>
		[EnumMember(Value = "scoring_boolean")]
		ScoringBoolean,
		/// <summary>
		/// Similar to scoring_boolean except scores are not computed. Instead, each matching document receives a constant
		///  score equal to the query’s boost. This rewrite method will hit too many clauses failure if it exceeds the 
		/// boolean query limit (defaults to 1024).
		/// </summary>
		[EnumMember(Value = "constant_score_boolean")]
		ConstantScoreBoolean,
		/// <summary>
		/// A rewrite method that first translates each term into should clause in boolean query, and keeps the scores 
		/// as computed by the query. This rewrite method only uses the top scoring terms so it will not overflow boolean
		///  max clause count. The N controls the size of the top scoring terms to use.
		/// </summary>
		[EnumMember(Value = "top_terms_N")]
		TopTermsN,
		/// <summary>
		/// A rewrite method that first translates each term into should clause in boolean query, but the scores are only 
		/// computed as the boost. This rewrite method only uses the top scoring terms so it will not overflow the boolean
		///  max clause count. The N controls the size of the top scoring terms to use.
		/// </summary>
		[EnumMember(Value = "top_terms_boost_N")]
		TopTermsBoostN,
		/// <summary>
		/// A rewrite method that first translates each term into should clause in boolean query, but all term queries compute
		///  scores as if they had the same frequency. In practice the frequency which is used is the maximum frequency of all
		///  matching terms. This rewrite method only uses the top scoring terms so it will not overflow boolean max clause count. 
		/// The N controls the size of the top scoring terms to use.
		/// </summary>
		[EnumMember(Value = "top_terms_blended_freqs_N")]
		TopTermsBlendedFreqsN
	}
}
