import { useState } from "react";

interface User {
  id: string;
  email: string;
  name: string;
  address: string;
  phoneNumber: string;
  role: string;
}

export default function GegevensInput(props: { user: User }) {
    const [input, setInput] = useState({ name: props.user.name, email: props.user.email, address: props.user.address, phoneNumber: props.user.phoneNumber });
    const handleChange = (e: { target: { name: any; value: any } }) => {
    setInput({ ...input, [e.target.name]: e.target.value });
    };
  return (
    <form>
      <input 
          className={"input"} 
          type="text" 
          name="naam"
          value={input.name} 
          placeholder="Naam"
          onChange={handleChange}
        />
        <input 
          className={"input"} 
          type="text" 
          name="email"
          value={input.email} 
          placeholder="Email@example.com"
          onChange={handleChange}
        />
        <p>Role: {props.user.role} </p>
        <input 
          className={"input"} 
          type="text" 
          name="address"
          value={input.address} 
          placeholder="Address"
          onChange={handleChange}
        />
        <input 
          className={"input"} 
          type="text" 
          name="phoneNumber"
          value={input.phoneNumber} 
          placeholder="1234567890"
          onChange={handleChange}
        />
    </form>
  )
}