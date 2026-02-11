import { useEffect, useState } from "react";
import FoodsApiService from "../services/FoodsApiService";

//TBD overwrites/gets all cookies to be fixed with more logic - not enough time
const getUser = () => document.cookie;
const setUser: () => string = () => {
  document.cookie = crypto.randomUUID();
  return document.cookie;
};

export default function useFood() {
  const [response, setResponse] = useState<any | null>(null);
  const [message, setMessage] = useState<string | null>(null);

  useEffect(() => {
    let clientId = getUser();
    if (!clientId) {
      clientId = setUser();
    }

    const getFoodsForClient = async (clientId: string) => {
      try {
        setMessage(null);
        const data = await FoodsApiService.getFood(clientId);
        setResponse(data);
      } catch (err) {
        setMessage(err instanceof Error ? err.message : String(err));
      }
    };

    getFoodsForClient(clientId);
  }, []);

  return { clientId: getUser(), response, message };
}
