export default function GegevensShow(props: any) {
    return (
    <form className={"userinfo-form"}>
        <p>Name:</p>
        <input 
            type="text" 
            name="naam"
            value={props.user.name} 
            placeholder="Naam"
            disabled
        />
        <p>Email:</p>
        <input 
          type="text" 
          name="email"
          value={props.user.email} 
          placeholder="Email@example.com"
          disabled
        />
        <p>Role:</p>
        <p className="role">{props.user.role} </p>
        <p>Address:</p>
        <input 
          type="text" 
          name="address"
          value={props.user.address} 
          placeholder="Address"
          disabled
        />
        <p>Phone Number:</p>
        <input 
          type="text" 
          name="phoneNumber"
          value={props.user.phoneNumber} 
          placeholder="1234567890"
          disabled
        />
        <div className={"userinfo-buttons"}>
          <button onClick={props.edit}>Edit</button>
          <div></div>
        </div>
    </form>
    )
  }