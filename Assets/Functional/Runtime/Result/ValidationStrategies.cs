using System;
using System.Collections.Generic;
using System.Linq;

namespace UFunctional
{
    public delegate Result<T> Validator<T>(T t);

    public static partial class F
    {
        // runs all validators, and fails when the first one fails
        public static Validator<T> FailFast<T>(IEnumerable<Validator<T>> validators) 
            => t => validators.Aggregate(
                Success(t), 
                (acc, validator) => acc.Bind(_ => validator(t)));

        // runs all validators, accumulating all validation errors (chapter 7)
        public static Validator<T> HarvestErrors<T>(IEnumerable<Validator<T>> validators)
           => t =>
           {
               var errors = validators
                .Map(validate => validate(t))
                .Bind(v => v.Match(
                   onError: errs => Some(errs),
                   onSuccess: _ => None))
                .ToList();

               return errors.Count == 0
                ? Success(t)
                : Error(errors.Flatten());
           };

        // runs all validators, accumulating all validation errors
        public static Validator<T> HarvestErrorsTr<T>
           (params Validator<T>[] validators)
           => t
           => validators
              .Traverse(validate => validate(t))
              .Map(_ => t);
    }
}
