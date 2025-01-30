import { useState } from "react";

import audi from "../assets/Audi.jpeg";
import bmw from "../assets/BMW-7er.jpg";
import koenigsegg from "../assets/koenigsegg-regera.jpg";	
import miata from "../assets/miata.jpg";
import porsche from "../assets/porsche2.png";


import "../styles/homepage.css";

export default function Index() {
  // Array of images to display in the courasel
  const images = [audi, bmw, koenigsegg, miata, porsche];
  // State to store the current image number
  const [imageNr, setImageNr] = useState<number>(0);
  // Function to change the image in the courasel
  function changeImage(incriment: number) {
    setImageNr((imageNr + incriment + images.length) % images.length);
  }

  return (
    <>
      {/* Banner text */}
      <div className="page-center banner-text">
        <h1>CarAndAll</h1>
        <p>Car, caravans and campers</p>
        {/* <button>Rent now!</button> */}
      </div>
      {/* Courasel to display the images */}
      <div className="image-courasel page-center">
        <img className="image-in-courasel" src={images[imageNr]} alt="Car images" />
        <div className="courasel-buttons">
          <button onClick={() => changeImage(-1)}>{"<"}</button>
          <button onClick={() => changeImage(1)}>{">"}</button>
        </div>
      </div>
    </>
  );
}