import type { FoodForClient } from "../types/responses";

class FoodsApiService {
  private baseUrl: string = "http://localhost:5294/api/foods";

  async getFood(
    clientId: string,
    retry: boolean = true,
  ): Promise<FoodForClient> {
    const response = await fetch(`${this.baseUrl}/${clientId}`, {
      method: "GET",
      headers: {
        "Content-Type": "application/json",
      },
    });

    switch (response.status) {
      case 200:
        return (await response.json()) as FoodForClient;
      case 202:
        throw new Error("Processing in progress");
      case 503:
        if (retry) {
          await new Promise((resolve) => setTimeout(resolve, 1000));
          return this.getFood(clientId, false);
        }
        throw new Error("Service unavailable");
      default:
        throw new Error("Failed to fetch food");
    }
  }
}

export default new FoodsApiService();
