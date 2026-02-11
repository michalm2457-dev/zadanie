interface FoodDisplayProps {
  foodName: string | null;
  message: string | null;
}

export default function FoodDisplay({ foodName, message }: FoodDisplayProps) {
  return (
    <>
      {!message ? (
        <h2>
          Food Name : <span className="food-result">{foodName}</span>
        </h2>
      ) : (
        <h2>{message}</h2>
      )}
    </>
  );
}
