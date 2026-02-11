import "./App.css";
import FoodDisplay from "./components/FoodDisplay";
import useFood from "./hooks/useFood";

function App() {
  const { clientId, response, message } = useFood();

  return (
    <>
      <h3>{clientId}</h3>
      <FoodDisplay
        foodName={response?.foodForCLientResponse?.foodName ?? null}
        message={message}
      />
    </>
  );
}

export default App;
